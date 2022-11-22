package main

import (
	"net/http"
)

const (
	hostPort = ":8090"
)

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

		w.Write([]byte(result))
	}
}

func main() {
	// initialize router
	router := http.NewServeMux()
	router.HandleFunc("/is_root", isRoot())
	http.ListenAndServe(hostPort, router)
}
