<?php
// This script validates user login credentials using the submitted username and password via the Post method from Login.cs
// The script connects to the MySQL database, retrieves the stored hash and salt for the given username, hashes the provided
// password with the stored salt, and compares it to the stored hash to authenticate the user.
    header('Content-Type: application/json');
    
    // Check if required POST data exists
    if (!isset($_POST['username']) || !isset($_POST['password'])) {
        echo json_encode(['success' => false, 'message' => 'Missing required fields']);
        exit;
    }

    $con = mysqli_connect('103-89-14-161.cloud-xip.com', 'root', 'DPWe3qN67TW3tkf', 'oral_medicine', 3306);
    if (!$con) {
        echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit;
    }

    $username = $_POST['username'];
    $password = $_POST['password'];

    // Input validation
    if (empty($username) || empty($password)) {
        echo json_encode(['success' => false, 'message' => 'Username and password cannot be empty']);
        exit;
    }

    $query = "SELECT hash, salt FROM users WHERE username = ?";
    $stmt = mysqli_prepare($con, $query);
    
    if ($stmt) {
        mysqli_stmt_bind_param($stmt, "s", $username);
        $result = mysqli_stmt_execute($stmt);
        
        if ($result) {
            $received = mysqli_stmt_get_result($stmt);
            if ($received && $row = mysqli_fetch_assoc($received)) {
                $storedHash = $row['hash'];
                $storedSalt = $row['salt'];
                
                // Generate hash for the provided password using the stored salt
                $computedHash = crypt($password, $storedSalt);
                
                if ($computedHash === $storedHash) {
                    echo json_encode(['success' => true, 'message' => 'User authenticated successfully']);
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