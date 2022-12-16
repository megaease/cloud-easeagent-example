package main

import (
	"net/http"
	"os"

	"github.com/megaease/easeagent-sdk-go/agent"
	"github.com/megaease/easeagent-sdk-go/plugins/zipkin"
)

const (
	localHostPort = ":8090"
)

// If you want to publish the `docker app` through the `cloud of megaease` and send the monitoring data to the `cloud`,
// please obtain the configuration file path through the environment variable `EASEAGENT_CONFIG`.
// We will pass it to you the `cloud configuration` file path.

// new tracing agent from yaml file and set host and port of Span.localEndpoint
// By default, use yamlFile="" is use easemesh.DefaultSpec() and Console Reporter for tracing.
// By default, use localHostPort="" is not set host and port of Span.localEndpoint.
var easeagent, _ = agent.NewWithOptions(agent.WithYAML(os.Getenv("EASEAGENT_CONFIG"), localHostPort))
var tracing = easeagent.GetPlugin(zipkin.Name).(zipkin.Tracing)

func isRoot() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		values := r.URL.Query()
		arg := values.Get("name")
		if arg == "" {
			w.Write([]byte("false"))
			return
		}
		result := ""
		if arg == "admin" {
			result = "true"
		} else {
			result = "false"
		}

		//send redis span
		redisSpan, _ := tracing.StartMWSpanFromCtx(r.Context(), "redis-get_key", zipkin.Redis)
		if endpoint, err := zipkin.NewEndpoint("redis-local_server", "127.0.0.1:8090"); err == nil {
			redisSpan.SetRemoteEndpoint(endpoint)
		}
		redisSpan.Finish()

		w.Write([]byte(result))
	}
}

func main() {
	// initialize router
	router := http.NewServeMux()
	router.HandleFunc("/is_root", isRoot())
	http.ListenAndServe(localHostPort, easeagent.WrapUserHandler(router))
}
