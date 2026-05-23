<?php
// This script validates user login credentials using the submitted username and password via the Post method from Login.cs
// The script connects to the MySQL database, retrieves the stored hash and salt for the given username, hashes the provided
// password with the stored salt, and compares it to the stored hash to authenticate the user.
    header('Content-Type: application/json');
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Headers: Content-Type');

    if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
        http_response_code(200);
        exit;
    }

    $rawInput = file_get_contents('php://input');
    $username = $_POST['username'] ?? null;
    $password = $_POST['password'] ?? null;

    if (($username === null || $password === null) && !empty($rawInput)) {
        parse_str($rawInput, $parsedInput);
        $username = $username ?? ($parsedInput['username'] ?? null);
        $password = $password ?? ($parsedInput['password'] ?? null);
    }

    // Check if required POST data exists
    if ($username === null || $password === null) {
        echo json_encode(['success' => false, 'message' => 'Missing required fields']);
        exit;
    }

    $con = mysqli_connect('103.89.14.188', 'root', 'GoDentalCougars66@!', 'oral_medicine', port: 3306);
    if (!$con) {
        echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit;
    }

    // Input validation
    if (empty($username) || empty($password)) {
        echo json_encode(['success' => false, 'message' => 'Username and password cannot be empty']);
        exit;
    }

    $query = "SELECT hash, salt, `type` AS user_type FROM users WHERE username = ?";
    $stmt = mysqli_prepare($con, $query);
    
    if ($stmt) {
        mysqli_stmt_bind_param($stmt, "s", $username);
        $result = mysqli_stmt_execute($stmt);
        
        if ($result) {
            $received = mysqli_stmt_get_result($stmt);
            if ($received && $row = mysqli_fetch_assoc($received)) {
                $storedHash = $row['hash'];
                $storedSalt = $row['salt'];
                $storedType = $row['user_type'];
                
                // Generate hash for the provided password using the stored salt
                $computedHash = crypt($password, $storedSalt);
                
                if ($computedHash === $storedHash) {
                    echo json_encode(['success' => true, 'message' => 'User authenticated successfully', 'type' => $storedType]);
                } else {
                    echo json_encode(['success' => false, 'message' => 'Invalid username or password']);
                }
            } else {
                echo json_encode(['success' => false, 'message' => 'Invalid username or password']);
            }
        } else {
            echo json_encode(['success' => false, 'message' => 'Failed to authenticate user']);
        }
        
        mysqli_stmt_close($stmt);
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
    }

    mysqli_close($con);
?>