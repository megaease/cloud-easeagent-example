<?php

if (preg_match('/\.(?:png|jpg|jpeg|gif)$/', $_SERVER["REQUEST_URI"])) {
    return false;    // serve the requested resource as-is.
}

use Easeagent\AgentBuilder;
use Easeagent\HTTP\HttpUtils;
use Zipkin\Endpoint;

require_once __DIR__ . '/vendor/autoload.php';

$agent = AgentBuilder::buildFromYaml(getenv('EASEAGENT_SDK_CONFIG_FILE'));

$agent->serverTransaction(function ($span) use ($agent) {
    // Open the table
    echo '<table border="1" cellspacing="0" cellpadding="0">';
    echo "<tr>";
    echo "<th>Name</th>";
    echo "<th>root</th>";
    echo "<th>Create Time</th>";
    echo "</tr>";

    $checkRootSpan = $agent->startClientSpan($span, "check-root");
    try {
        // $headers = $agent->injectorHeaders($checkRootSpan);
        /* HTTP Request to the backend */
        // $httpClient = new Client();
        // $request = new \GuzzleHttp\Psr7\Request('POST', 'localhost:9000', $headers);
        // $response = $httpClient->send($request);
        /* Save Request info */
        // HttpUtils::finishSpan($childSpan, $request->getMethod(), $request->getUri()->getPath(), $response->getStatusCode());
        HttpUtils::finishSpan($checkRootSpan, "GET", "/checkroot", 200);
    } catch (Exception $e) {
        $checkRootSpan->setError($e);
        $checkRootSpan->finish();
    }

    $mysqlSpan = $agent->startMiddlewareSpan($span,  'user:get_list:mysql_query', Type::MySql);
    $mysqlSpan->setRemoteEndpoint(Endpoint::create("mysql-user", "0.0.0.0", null, 8081));
    usleep(50000);
    $mysqlSpan->finish();

    // Output a row
    echo "<tr>";
    echo "<td>admin</td>";
    echo "<td>true</td>";
    echo "<td>2022</td>";
    echo "</tr>";

    echo "</table>";
});
