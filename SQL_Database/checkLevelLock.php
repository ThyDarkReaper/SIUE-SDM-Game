<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

if (!isset($_GET['levelID']) && !isset($_GET['level'])) {
    echo json_encode(['success' => false, 'message' => 'Missing levelID parameter']);
    exit;
}

$rawLevelID = isset($_GET['levelID']) ? $_GET['levelID'] : $_GET['level'];
if (!is_numeric($rawLevelID)) {
    echo json_encode(['success' => false, 'message' => 'Invalid levelID parameter']);
    exit;
}

$levelID = (int) $rawLevelID;

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$query = "SELECT isLocked FROM levelLock WHERE levelID = ?";
$stmt = mysqli_prepare($con, $query);
if ($stmt) {
    mysqli_stmt_bind_param($stmt, "i", $levelID);
    $result = mysqli_stmt_execute($stmt);
    if ($result) {
        $received = mysqli_stmt_get_result($stmt);
        if ($received && $row = mysqli_fetch_assoc($received)) {
            $isLocked = (bool) $row['isLocked'];
            echo json_encode(['success' => true, 'levelID' => $levelID, 'isLocked' => $isLocked]);
        } else {
            echo json_encode(['success' => false, 'message' => 'Level not found']);
        }
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to retrieve level lock status']);
    }
    mysqli_stmt_close($stmt);
} else {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
}

mysqli_close($con);
?>