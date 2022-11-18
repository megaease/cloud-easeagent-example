<?php

if (preg_match('/\.(?:png|jpg|jpeg|gif)$/', $_SERVER["REQUEST_URI"])) {
    return false;    // serve the requested resource as-is.
}

use Easeagent\AgentBuilder;
use Easeagent\HTTP\HttpUtils;
use GuzzleHttp\Client;

require_once __DIR__ . '/vendor/autoload.php';

$configPath = getenv('EASEAGENT_SDK_CONFIG_FILE');
if ($configPath === false) {
    $configPath = __DIR__ . '/agent_frontend.yml';
}
echo "<p>configPath: " . $configPath . "</p>";
$agent = AgentBuilder::buildFromYaml($configPath);
$agent->serverTransaction(function ($span) use ($agent) {
    $useListURL = getenv('USER_LIST_URL');
    if ($useListURL === false) {
        $useListURL = "http://127.0.0.1:18888/user/list";
    }
    $checkRootURL = getenv('CHECK_ROOT_URL');
    if ($checkRootURL === false) {
        $checkRootURL = "http://127.0.0.1:8090/is_root";
    }

    $httpClient = new Client();
    $useListSpan = $agent->startClientSpan($span, "get-user-list");
    $request = new \GuzzleHttp\Psr7\Request('GET', $useListURL, $agent->injectorHeaders($useListSpan));
    $response = $httpClient->send($request);
    HttpUtils::finishSpan($useListSpan, $request->getMethod(), $request->getUri()->getPath(), $response->getStatusCode());
    $json = $response->getBody();

    $data =  json_decode($json);

    if (count($data)) {
        // Open the table
        echo '<table border="1" cellspacing="0" cellpadding="0">';
        echo "<tr>";
        echo "<th>Name</th>";
        echo "<th>root</th>";
        echo "<th>Create Time</th>";
        echo "</tr>";

        // Cycle through the array
        foreach ($data as $stand) {

            $checkRootSpan = $agent->startClientSpan($span, "check-root");
            $request = new \GuzzleHttp\Psr7\Request('GET', $checkRootURL . "?name=" . $stand->name, $agent->injectorHeaders($checkRootSpan));
            $isRoot = "unknow";
            try {
                $response = $httpClient->send($request);
                if ($response->getStatusCode() == 200) {
                    $isRoot = $response->getBody();
                }
                HttpUtils::finishSpan($checkRootSpan, $request->getMethod(), $request->getUri()->getPath(), $response->getStatusCode());
            } catch (Exception $e) {
                $checkRootSpan->setError($e);
                $checkRootSpan->finish();
            }
            // Output a row
            echo "<tr>";
            echo "<td>" . $stand->name . "</td>";
            echo "<td>" . $isRoot . "</td>";
            echo "<td>" . $stand->createTime . "</td>";
            echo "</tr>";
        }

        echo "</table>";
    }
});
