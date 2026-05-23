<?php

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

$rawInput = file_get_contents('php://input');
$username = $_POST['username'] ?? null;
$charID = $_POST['charID'] ?? null;

if (($username === null || $charID === null) && !empty($rawInput)) {
    parse_str($rawInput, $parsedInput);
    $username = $username ?? ($parsedInput['username'] ?? null);
    $charID = $charID ?? ($parsedInput['charID'] ?? null);
}

if ($username === null || $charID === null) {
    echo json_encode(['success' => false, 'message' => 'Missing required fields']);
    exit;
}

$con = mysqli_connect('103.89.14.188', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

if (empty($username) || !is_numeric($charID)) {
    echo json_encode(['success' => false, 'message' => 'Invalid input data']);
    mysqli_close($con);
    exit;
}

$charID = (int)$charID;
$query = "UPDATE users SET characterID = ? WHERE username = ?";
$stmt = mysqli_prepare($con, $query);

if (!$stmt) {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare statement: ' . mysqli_error($con)]);
    mysqli_close($con);
    exit;
}

mysqli_stmt_bind_param($stmt, "is", $charID, $username);
$result = mysqli_stmt_execute($stmt);

if ($result) {
    $affectedRows = mysqli_stmt_affected_rows($stmt);
    if ($affectedRows === 1) {
        echo json_encode(['success' => true, 'message' => 'Character ID updated successfully']);
    } else {
        echo json_encode(['success' => false, 'message' => 'User not found or no changes made']);
    }
} else {
    echo json_encode(['success' => false, 'message' => 'Failed to execute query: ' . mysqli_stmt_error($stmt)]);
}

mysqli_stmt_close($stmt);
mysqli_close($con);

