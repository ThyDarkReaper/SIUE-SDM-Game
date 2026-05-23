<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');

if (!function_exists('mysqli_connect')) {
    echo json_encode(['success' => false, 'message' => 'mysqli extension is not enabled on this server']);
    exit;
}

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'DB connection failed: ' . mysqli_connect_error()]);
    exit;
}

$query = "SELECT u.username, s.level1score, s.level2score, s.level3score, s.level4score, s.level5score, s.level6score FROM users u INNER JOIN student s ON s.userID = u.id ORDER BY u.username ASC";
$result = mysqli_query($con, $query);

if (!$result) {
    echo json_encode(['success' => false, 'message' => 'Query failed: ' . mysqli_error($con)]);
    mysqli_close($con);
    exit;
}

$users = [];
while ($row = mysqli_fetch_assoc($result)) {
    $users[] = $row;
}

echo json_encode(['success' => true, 'users' => $users]);
mysqli_close($con);
