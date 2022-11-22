package main

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"os"

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
	fmt.Println("config path : " + filePath)
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
		redisSpan, _ := zipkinAgent.StartMWSpanFromCtx(r.Context(), "redis-get_key", zipkin.Redis)
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
	http.ListenAndServe(hostPort, easeagent.WrapUserHandler(router))
}
