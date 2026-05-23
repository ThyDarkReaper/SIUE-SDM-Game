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

if ($username === null && !empty($rawInput)) {
    parse_str($rawInput, $parsedInput);
    $username = $parsedInput['username'] ?? null;
}

if ($username === null) {
    echo json_encode(['success' => false, 'message' => 'Missing required fields']);
    exit;
}

$con = mysqli_connect('103.89.14.188', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

if (empty($username)) {
    echo json_encode(['success' => false, 'message' => 'Username cannot be empty']);
    exit;
}

$query = "SELECT characterID FROM users WHERE username = ?";
$stmt = mysqli_prepare($con, $query);
if ($stmt) {
    mysqli_stmt_bind_param($stmt, "s", $username);
    $result = mysqli_stmt_execute($stmt);
    
    if ($result) {
        $received = mysqli_stmt_get_result($stmt);
        if ($received && $row = mysqli_fetch_assoc($received)) {
            echo json_encode(['success' => true, 'charID' => $row['characterID']]);
        } else {
            echo json_encode(['success' => false, 'message' => 'User not found']);
        }
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to execute query']);
    }
    
    mysqli_stmt_close($stmt);
} else {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
}
mysqli_close($con);
?>