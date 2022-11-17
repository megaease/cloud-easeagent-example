package main

import (
	"context"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"os"
	"time"

	redis "github.com/go-redis/redis/v9"

	"github.com/megaease/easeagent-sdk-go/agent"
	"github.com/megaease/easeagent-sdk-go/plugins"
	"github.com/megaease/easeagent-sdk-go/plugins/zipkin"
	"gopkg.in/yaml.v2"
)

const (
	hostPort = ":8090"
)

var easeagent = newAgent(hostPort)
var zipkinAgent = easeagent.GetPlugin(zipkin.NAME).(*zipkin.Zipkin)
var rdb = createRedisClient()

// new agent
func newAgent(hostport string) *agent.Agent {
	fileConfigPath := os.Getenv("EASEAGENT_SDK_CONFIG_FILE")
	var zipkinSpec zipkin.Spec
	if fileConfigPath == "" {
		zipkinSpec = zipkin.DefaultSpec().(zipkin.Spec)
		zipkinSpec.OutputServerURL = "" // report to log when output server is ""
	} else {
		spec, err := LoadSpecFromYamlFile(fileConfigPath)
		exitfIfErr(err, "new zipkin spec fail: %v", err)
		zipkinSpec = *spec
	}
	zipkinSpec.Hostport = hostport
	agent, err := agent.New(&agent.Config{
		Plugins: []plugins.Spec{
			zipkinSpec,
		},
	})
	exitfIfErr(err, "new agent fail: %v", err)
	return agent
}

func exitfIfErr(err error, format string, args ...interface{}) {
	if err == nil {
		return
	}
	fmt.Fprintf(os.Stderr, format+"\n", args...)
	os.Exit(1)
}

func LoadSpecFromYamlFile(filePath string) (*zipkin.Spec, error) {
	buff, err := ioutil.ReadFile(filePath)
	if err != nil {
		return nil, fmt.Errorf("read config file :%s failed: %v", filePath, err)
	}
	fmt.Println(string(buff))
	var body map[string]interface{}
	err = yaml.Unmarshal(buff, &body)
	if err != nil {
		return nil, fmt.Errorf("unmarshal yaml file %s to map failed: %v",
			filePath, err)
	}

	bodyJson, err := json.Marshal(body)
	if err != nil {
		return nil, fmt.Errorf("marshal yaml file %s to json failed: %v",
			filePath, err)
	}
	var spec zipkin.Spec
	err = json.Unmarshal(bodyJson, &spec)
	if err != nil {
		return nil, fmt.Errorf("unmarshal %s to %T failed: %v", bodyJson, spec, err)
	}
	spec.KindField = zipkin.Kind
	spec.NameField = zipkin.NAME
	return &spec, nil
}

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
	span, _ := zipkinAgent.StartMWSpanFromCtx(r.Context(), "get", zipkin.Redis)
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
	addSpan, _ := zipkinAgent.StartMWSpanFromCtx(r.Context(), "set", zipkin.Redis)
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
	http.ListenAndServe(hostPort, easeagent.WrapUserHandler(router))
}
