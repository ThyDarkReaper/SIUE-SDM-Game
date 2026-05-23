<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: Content-Type');
header('Access-Control-Allow-Methods: POST, OPTIONS');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

$mysqli = new mysqli('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if ($mysqli->connect_error) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed']);
    exit;
}

$rawInput = file_get_contents('php://input');
$username = $_POST['username'] ?? null;

if ($username === null && !empty($rawInput)) {
    parse_str($rawInput, $parsedInput);
    $username = $parsedInput['username'] ?? null;
}

$username = $username !== null ? trim($username) : null;
if ($username === null || $username === '') {
    echo json_encode(['success' => false, 'message' => 'Username not provided']);
    $mysqli->close();
    exit;
}

$salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
$hash = crypt('test123', $salt);

if ($hash === false || strlen($hash) < 10) {
    echo json_encode(['success' => false, 'message' => 'Password hashing failed']);
    $mysqli->close();
    exit;
}

$mysqli->begin_transaction();

try {
    $userStmt = $mysqli->prepare("INSERT INTO users (username, hash, salt, type) VALUES (?, ?, ?, 'student')");
    if (!$userStmt) {
        throw new Exception('Failed to prepare users insert');
    }

    $userStmt->bind_param('sss', $username, $hash, $salt);
    if (!$userStmt->execute() || $userStmt->affected_rows !== 1) {
        $userStmt->close();
        throw new Exception('Failed to insert user');
    }

    $newUserId = (int) $mysqli->insert_id;
    $userStmt->close();

    $studentStmt = $mysqli->prepare("INSERT INTO student (userid) VALUES (?)");
    if (!$studentStmt) {
        throw new Exception('Failed to prepare student insert');
    }

    $studentStmt->bind_param('i', $newUserId);
    if (!$studentStmt->execute() || $studentStmt->affected_rows !== 1) {
        $studentStmt->close();
        throw new Exception('Failed to insert student record');
    }

    $studentStmt->close();
    $mysqli->commit();
    echo json_encode(['success' => true, 'message' => 'Player added successfully']);
} catch (Exception $e) {
    $mysqli->rollback();
    echo json_encode(['success' => false, 'message' => $e->getMessage()]);
}

$mysqli->close();

    