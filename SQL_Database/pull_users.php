<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$query = "SELECT username, level1score, level2score, level3score, level4score, level5score, level6score FROM users ORDER BY username ASC";
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
