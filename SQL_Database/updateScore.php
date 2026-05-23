<?php

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: Content-Type');
header('Access-Control-Allow-Methods: POST, OPTIONS');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

$rawInput = file_get_contents('php://input');
$username = $_POST['username'] ?? null;
$score = $_POST['score'] ?? null;
$level = $_POST['level'] ?? null;

if (($username === null || $score === null || $level === null) && !empty($rawInput)) {
    parse_str($rawInput, $parsedInput);
    $username = $username ?? ($parsedInput['username'] ?? null);
    $score = $score ?? ($parsedInput['score'] ?? null);
    $level = $level ?? ($parsedInput['level'] ?? null);
}

if ($username === null || $score === null || $level === null) {
    echo json_encode(['success' => false, 'message' => 'Missing required fields']);
    exit;
}

$username = trim($username);
$rawScore = (string) $score;
$level = trim((string) $level);

if ($username === '' || !is_numeric($rawScore) || $level === '') {
    echo json_encode(['success' => false, 'message' => 'Invalid input data']);
    exit;
}

$score = (float) $rawScore;

// Only allow known level score columns to prevent SQL injection through column names.
$allowedColumns = [
    'level1score',
    'level2score',
    'level3score',
    'level4score',
    'level5score',
    'level6score'
];

if (!in_array($level, $allowedColumns, true)) {
    echo json_encode(['success' => false, 'message' => 'Invalid level field']);
    exit;
}

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$userQuery = 'SELECT id FROM users WHERE username = ? LIMIT 1';
$userStmt = mysqli_prepare($con, $userQuery);
if (!$userStmt) {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare user lookup']);
    mysqli_close($con);
    exit;
}

mysqli_stmt_bind_param($userStmt, 's', $username);
if (!mysqli_stmt_execute($userStmt)) {
    echo json_encode(['success' => false, 'message' => 'Failed to execute user lookup']);
    mysqli_stmt_close($userStmt);
    mysqli_close($con);
    exit;
}

$userResult = mysqli_stmt_get_result($userStmt);
$userRow = $userResult ? mysqli_fetch_assoc($userResult) : null;
mysqli_stmt_close($userStmt);

if (!$userRow || !isset($userRow['id'])) {
    echo json_encode(['success' => false, 'message' => 'User not found']);
    mysqli_close($con);
    exit;
}

$userId = (int) $userRow['id'];

$updateQuery = "UPDATE student SET {$level} = ? WHERE userid = ?";
$updateStmt = mysqli_prepare($con, $updateQuery);
if (!$updateStmt) {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare score update']);
    mysqli_close($con);
    exit;
}

mysqli_stmt_bind_param($updateStmt, 'di', $score, $userId);
$updateResult = mysqli_stmt_execute($updateStmt);

if (!$updateResult) {
    echo json_encode(['success' => false, 'message' => 'Failed to execute score update']);
    mysqli_stmt_close($updateStmt);
    mysqli_close($con);
    exit;
}

$affectedRows = mysqli_stmt_affected_rows($updateStmt);
mysqli_stmt_close($updateStmt);
mysqli_close($con);

if ($affectedRows >= 0) {
    echo json_encode(['success' => true, 'message' => "Level {$level} score updated successfully", 'changed' => $affectedRows > 0]);
} else {
    echo json_encode(['success' => false, 'message' => 'Score update failed']);
}

