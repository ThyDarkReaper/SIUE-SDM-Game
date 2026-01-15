<?php
    error_reporting(E_ALL);
    ini_set('display_errors', 0); // Don't display errors in production, but log them
    
// This script changes the password for the user using the submitted username and newPassword via the POST method from ChangePassword.cs
// The script connects to the MYSQL database, validates the input, hashes the new password with a salt, and updates the user's password in the database.

    header('Content-Type: application/json');
    
    try {
        // Check if required POST data exists
        if (!isset($_POST['username']) || !isset($_POST['newPassword'])) {
            echo json_encode(['success' => false, 'message' => 'Missing required fields']);
            exit;
        }

        // Validate input first
        $username = trim($_POST['username']);
        $newPassword = trim($_POST['newPassword']);

        if (empty($username) || empty($newPassword)) {
            echo json_encode(['success' => false, 'message' => 'Username and password cannot be empty']);
            exit;
        }

        // Database connection with error handling
        $con = @mysqli_connect('103.89.14.161', 'root', 'DPWe3qN67TW3tkf', 'oral_medicine', 3306);
        
        if (!$con) {
            echo json_encode(['success' => false, 'message' => 'Database connection failed']);
            exit;
        }

        // Generate salt and hash
        $salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
        $hashedPassword = crypt($newPassword, $salt);

        // Check if crypt succeeded
        if ($hashedPassword === false || strlen($hashedPassword) < 10) {
            mysqli_close($con);
            echo json_encode(['success' => false, 'message' => 'Password hashing failed']);
            exit;
        }

        // Prepare and execute query
        $query = "UPDATE users SET salt = ?, hash = ? WHERE username = ?";
        $stmt = mysqli_prepare($con, $query);
        
        if ($stmt) {
            mysqli_stmt_bind_param($stmt, "sss", $salt, $hashedPassword, $username);
            $result = mysqli_stmt_execute($stmt);
            
            if ($result) {
                $affectedRows = mysqli_stmt_affected_rows($stmt);
                if ($affectedRows > 0) {
                    echo json_encode(['success' => true, 'message' => 'Password updated successfully']);
                } else {
                    echo json_encode(['success' => false, 'message' => 'User not found or password unchanged']);
                }
            } else {
                echo json_encode(['success' => false, 'message' => 'Failed to execute update']);
            }
            
            mysqli_stmt_close($stmt);
        } else {
            echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
        }

        mysqli_close($con);
        
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'message' => 'An error occurred while processing your request']);
    }
?>