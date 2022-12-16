package main

import (
	"context"
	"fmt"
	"net/http"
	"os"
	"time"

	redis "github.com/go-redis/redis/v9"

	"github.com/megaease/easeagent-sdk-go/agent"
	"github.com/megaease/easeagent-sdk-go/plugins/zipkin"
)

const (
	localHostPort = ":8090" // your server host and port for
)

// If you want to publish the `docker app` through the `cloud of megaease` and send the monitoring data to the `cloud`,
// please obtain the configuration file path through the environment variable `EASEAGENT_CONFIG`.
// We will pass it to you the `cloud configuration` file path.

// new tracing agent from yaml file and set host and port of Span.localEndpoint
// By default, use yamlFile="" is use easemesh.DefaultSpec() and Console Reporter for tracing.
// By default, use localHostPort="" is not set host and port of Span.localEndpoint.
var easeagent, _ = agent.NewWithOptions(agent.WithYAML(os.Getenv("EASEAGENT_CONFIG"), localHostPort))
var tracing = easeagent.GetPlugin(zipkin.Name).(zipkin.Tracing)
var rdb = createRedisClient()

func createRedisClient() *redis.Client {
	hostAndPort := os.Getenv("REDIS_HOST_AND_PORT")
	if hostAndPort == "" {
		hostAndPort = "localhost:6379"
	}
	password := os.Getenv("REDIS_PASSWORD")
	if hostAndPort == "" {
		hostAndPort = ""
	}
	return redis.NewClient(&redis.Options{
		Addr:     hostAndPort,
		Password: password, // no password set
		DB:       0,        // use default DB
	})
}

func isRoot() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		values := r.URL.Query()
		arg := values.Get("name")
		if arg == "" {
			w.Write([]byte("false"))
			return
		}
		result, err := getOrPushToRedis(r, arg, func() string {
			if arg == "admin" {
				return "true"
			} else {
				return "false"
			}
		})
		if err != nil {
			w.WriteHeader(500)
			return
		}
		w.Write([]byte(result))
	}
}

func getOrPushToRedis(r *http.Request, key string, valueSuppler func() string) (string, error) {
	ctx := context.Background()
	span, _ := tracing.StartMWSpanFromCtx(r.Context(), "get", zipkin.Redis)
	if endpoint, err := zipkin.NewEndpoint("redis-user-admin", rdb.Options().Addr); err == nil {
		span.SetRemoteEndpoint(endpoint)
	}
	value := rdb.Get(ctx, key)
	if value.Err() == nil {
		redisResult, err := value.Result()
		if err != nil {
			span.Tag("error", err.Error())
			span.Finish()
			return "", fmt.Errorf("get result from redis error %v", err)
		}
		span.Finish()
		return redisResult, nil
	} else {
		span.Annotate(time.Now(), "can not found result from redis.")
		span.Finish()
	}
	result := valueSuppler()
	addSpan, _ := tracing.StartMWSpanFromCtx(r.Context(), "set", zipkin.Redis)
	if endpoint, err := zipkin.NewEndpoint("redis-user-admin", rdb.Options().Addr); err == nil {
		addSpan.SetRemoteEndpoint(endpoint)
	}
	status := rdb.Set(ctx, key, result, time.Second)
	if status.Err() != nil {
		addSpan.Tag("error", status.Err().Error())
		addSpan.Finish()
		return "", fmt.Errorf("set redis error %v", status.Err())
	}
	addSpan.Finish()
	return result, nil
}

func main() {
	// initialize router
	router := http.NewServeMux()
	router.HandleFunc("/is_root", isRoot())
	http.ListenAndServe(localHostPort, easeagent.WrapUserHandler(router))
}
