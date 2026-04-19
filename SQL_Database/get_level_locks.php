<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$query = "SELECT levelID, isLocked FROM levelLock ORDER BY levelID ASC";
$result = mysqli_query($con, $query);

if (!$result) {
    echo json_encode(['success' => false, 'message' => 'Query failed: ' . mysqli_error($con)]);
    mysqli_close($con);
    exit;
}

$locks = [];
while ($row = mysqli_fetch_assoc($result)) {
    $locks[] = [
        'levelID'  => (int) $row['levelID'],
        'isLocked' => (bool) $row['isLocked']
    ];
}

echo json_encode(['success' => true, 'locks' => $locks]);
mysqli_close($con);
