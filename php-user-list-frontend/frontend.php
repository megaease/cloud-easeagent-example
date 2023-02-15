<?php

use Zipkin\Span;

if (preg_match('/\.(?:png|jpg|jpeg|gif)$/', $_SERVER["REQUEST_URI"])) {
    return false;    // serve the requested resource as-is.
}

use Easeagent\Agent;
use Easeagent\AgentBuilder;
use Easeagent\HTTP\HttpUtils;
use GuzzleHttp\Client;

require_once __DIR__ . '/vendor/autoload.php';


function callServer(Client $client, string $url, Agent $agent, Span $parent, string $spanName): string
{
    $span = $agent->startClientSpan($parent, $spanName);
    $request = new \GuzzleHttp\Psr7\Request('GET', $url, $agent->injectorHeaders($span));
    $result = "";
    try {
        $response = $client->send($request);
        if ($response->getStatusCode() == 200) {
            $result = $response->getBody();
            if ($result == "") {
                $result = "success";
            }
        }
        HttpUtils::finishSpan($span, $request->getMethod(), $request->getUri()->getPath(), $response->getStatusCode());
    } catch (Exception $e) {
        $span->setError($e);
        $span->finish();
    }
    return $result;
}

function convertUrlQuery($query)
{

    $queryParts = explode('&', $query);

    $params = array();

    foreach ($queryParts as $param) {

        $item = explode('=', $param);

        $params[$item[0]] = $item[1];
    }

    return $params;
}

function callNet(Agent $agent, Span $span, Client $client): bool
{
    if (!isset($_SERVER['QUERY_STRING'])) {
        return false;
    }
    $url = 'http://' . $_SERVER['SERVER_NAME'] . ':' . $_SERVER["SERVER_PORT"] . $_SERVER["REQUEST_URI"];
    $params = convertUrlQuery(parse_url($url)["query"]);
    if (!isset($params["callMethod"]) || !isset($params["name"])) {
        return false;
    }
    $name = $params["name"];
    $netURL = getenv('NET_URL');
    if ($netURL === false) {
        $netURL = "http://localhost:7116/";
    }
    $callUrl = "";
    if ($params["callMethod"] === "add") {
        $callUrl = $netURL . "user?name=" . $name;
    } else if ($params["callMethod"] === "delete") {
        $callUrl = $netURL . "deleteUser?name=" . $name;
    } else {
        echo "not method to call: " . $params["callMethod"];
        return true;
    }
    $result = callServer($client, $callUrl, $agent, $span, "call .NET");
    if ($result == "") {
        echo "call url fail:" . $callUrl;
    } else {
        echo $result;
    }
    return true;
}

$agent = AgentBuilder::buildFromYaml(getenv('EASEAGENT_CONFIG'));
$agent->serverReceive(function ($span) use ($agent) {
    $useListURL = getenv('USER_LIST_URL');
    if ($useListURL === false) {
        $useListURL = "http://127.0.0.1:18888/user/list";
    }
    $checkRootURL = getenv('CHECK_ROOT_URL');
    if ($checkRootURL === false) {
        $checkRootURL = "http://127.0.0.1:8090/is_root";
    }
    $httpClient = new Client();
    if (callNet($agent, $span, $httpClient)) {
        return;
    }
    $json = callServer($httpClient, $useListURL, $agent, $span, "get-user-list");
    $data =  [];
    if ($json == "") {
        echo "call " . $useListURL . " fail.";
        return;
    } else {
        $data =  json_decode($json);
    }


    echo '<script type="text/javascript">',
    '
    function callUrl(url) {
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function() {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200){
                alert(xmlHttp.responseText);
                window.location.reload(1);
            }
        }
        xmlHttp.open("GET", url, true); // true for asynchronous 
        xmlHttp.send(null);
    }
    function addUser() {
        var name = document.getElementById("fname").value
        callUrl("/?callMethod=add&name=" + name);
    }
    function deleteUser(name) {
        callUrl("/?callMethod=delete&name=" + name);
    }
    ',
    '</script><br/>';
    // xmlHttp.open("GET", "' . $netURL . 'deleteUser?name=" + name, true); // true for asynchronous 
    if (count($data)) {
        // Open the table
        echo '<table border="1" cellspacing="0" cellpadding="0">';
        echo "<tr>";
        echo "<th>Name</th>";
        echo "<th>root</th>";
        echo "<th>Create Time</th>";
        echo "<th>Action</th>";
        echo "</tr>";

        // Cycle through the array
        foreach ($data as $stand) {

            $isRoot = callServer($httpClient, $checkRootURL . "?name=" . $stand->name, $agent, $span, "check-root");
            if ($isRoot == "") {
                $isRoot = "unknow";
            }

            // Output a row
            echo "<tr>";
            echo "<td>" . $stand->name . "</td>";
            echo "<td>" . $isRoot . "</td>";
            echo "<td>" . $stand->createTime . "</td>";
            echo '<td><input type = "button" onclick = "deleteUser(\'' . $stand->name . '\')" value = "DELETE"/></td>';
            echo "</tr>";
        }
        echo '<tr>
        <td><input type="text" id="fname" name="fname"></td>
        <td></td>
        <td></td>
        <td><input type = "button" onclick = "addUser()" value = "ADD"/></td>
        </tr>';

        echo "</table>";
    }
});
