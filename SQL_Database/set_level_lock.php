<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    echo json_encode(['success' => false, 'message' => 'Only POST requests are allowed']);
    exit;
}

$input = json_decode(file_get_contents('php://input'), true);

if (!isset($input['levelID']) || !isset($input['isLocked'])) {
    echo json_encode(['success' => false, 'message' => 'Missing required fields: levelID and isLocked']);
    exit;
}

$levelID  = (int) $input['levelID'];
$isLocked = $input['isLocked'] ? 1 : 0;

if ($levelID < 1 || $levelID > 6) {
    echo json_encode(['success' => false, 'message' => 'levelID must be between 1 and 6']);
    exit;
}

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$stmt = mysqli_prepare($con, "UPDATE levelLock SET isLocked = ? WHERE levelID = ?");
if (!$stmt) {
    echo json_encode(['success' => false, 'message' => 'Prepare failed: ' . mysqli_error($con)]);
    mysqli_close($con);
    exit;
}

mysqli_stmt_bind_param($stmt, 'ii', $isLocked, $levelID);
$result = mysqli_stmt_execute($stmt);

if (!$result) {
    echo json_encode(['success' => false, 'message' => 'Update failed: ' . mysqli_stmt_error($stmt)]);
    mysqli_stmt_close($stmt);
    mysqli_close($con);
    exit;
}

$affected = mysqli_stmt_affected_rows($stmt);
mysqli_stmt_close($stmt);
mysqli_close($con);

if ($affected === 0) {
    echo json_encode(['success' => false, 'message' => 'No row found for levelID ' . $levelID . '. Insert the row first.']);
    exit;
}

echo json_encode(['success' => true, 'levelID' => $levelID, 'isLocked' => (bool) $isLocked]);
