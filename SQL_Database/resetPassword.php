<?php
    header('Content-Type: application/json');
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Headers: Content-Type');

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

    if ($username !== null) {

        // Update the password for the specified username
        $stmt = $mysqli->prepare("UPDATE users SET password = 'test123' WHERE username = ?");
        $stmt->bind_param('s', $username);
        $stmt->execute();

        if ($stmt->affected_rows > 0) {
            echo json_encode(['success' => true, 'message' => 'Password reset successfully']);
        } else {
            echo json_encode(['success' => false, 'message' => 'Player not found']);
        }
        $stmt->close();
    } else {
        echo json_encode(['success' => false, 'message' => 'Username not provided']);
    }

    $mysqli->close();
?>