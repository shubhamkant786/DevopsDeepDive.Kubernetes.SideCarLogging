References: 
https://andrewlock.net/writing-logs-to-elasticsearch-with-fluentd-using-serilog-in-asp-net-core/
https://oneuptime.com/blog/post/2026-02-09-sidecar-containers-log-shipping/view

First:
podman compose --file ./docker-compose.yml up --build --detach

Successfully tagged shubhamkantdocker786/sidecarloggingweatherapi:1.0.0
Successfully tagged shubhamkantdocker786/sidecarloggingweatherapi:latest
Successfully tagged shubhamkantdocker786/sidecarloggingweatherapi
[+] up 4/4
 ✔ Image shubhamkantdocker786/sidecarloggingweatherapi            Built                                            21.2s
 ✔ Container devopsdeepdive.kubernetes.sidecarlogging.weather.api Started                                           0.6s
 ✔ Container elasticsearch                                        Healthy                                          22.8s
 ✔ Container kibana                                               Started                                           0.5s

This will start the elasticsearch, kibana, fluentd, and your application containers in detached mode.
elasticsearch will be available at http://localhost:9200 and kibana at http://localhost:5601.
http://localhost:5601/app/kibana

Second:
Push the image to docker hub using the following command:
podman push docker.io/shubhamkantdocker786/sidecarloggingweatherapi:latest

Third:
Create helm chart for the application using the following command:
helm create sidecarloggingweatherapi

Fourth:
Update the values.yaml file in the helm chart to include the image name and tag for your application.
Update the deployment.yaml file in the helm chart to include the sidecar container for fluentd.
Updathe the configmap.yaml file in the helm chart to include the fluentd configuration for shipping logs to elasticsearch.
Note: Choose the logfile path given in the serilog file sink

First check the chart created are good
helm lint ./sidecarloggingweatherapi

Now deploy chart to K8
helm install weather-sidecar-fluentd-r1 ./sidecarloggingweatherapi
NAME: weather-sidecar-fluentd-r1
LAST DEPLOYED: Sat Sep 12 21:11:57 2026
NAMESPACE: default
STATUS: deployed
REVISION: 1
DESCRIPTION: Install complete
NOTES:
1. Get the application URL by running these commands:
  export POD_NAME=$(kubectl get pods --namespace default -l "app.kubernetes.io/name=sidecarloggingweatherapi,app.kubernetes.io/instance=weather-sidecar-fluentd-r1" -o jsonpath="{.items[0].metadata.name}")
  export CONTAINER_PORT=$(kubectl get pod --namespace default $POD_NAME -o jsonpath="{.spec.containers[0].ports[0].containerPort}")
  echo "Visit http://127.0.0.1:8080 to use your application"
  kubectl --namespace default port-forward $POD_NAME 8080:$CONTAINER_PORT



