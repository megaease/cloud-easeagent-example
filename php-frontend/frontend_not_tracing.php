<?php
require_once __DIR__ . '/vendor/autoload.php';

if (preg_match('/\.(?:png|jpg|jpeg|gif)$/', $_SERVER["REQUEST_URI"])) {
    return false;    // serve the requested resource as-is.
}

// Open the table
echo '<table border="1" cellspacing="0" cellpadding="0">';
echo "<tr>";
echo "<th>Name</th>";
echo "<th>root</th>";
echo "<th>Create Time</th>";
echo "</tr>";

// Output a row
echo "<tr>";
echo "<td>admin</td>";
echo "<td>true</td>";
echo "<td>2022</td>";
echo "</tr>";

echo "</table>";
