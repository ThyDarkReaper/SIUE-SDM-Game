<?php
    header('Content-Type: application/json');
    
    // Check if required POST data exists
    if (!isset($_POST['username']) || !isset($_POST['newPassword'])) {
        echo json_encode(['success' => false, 'message' => 'Missing required fields']);
        exit;
    }

    $con = mysqli_connect('localhost', 'root', 'root', 'oral_medicine', 3306);
    if (!$con) {
        echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit;
    }

    $username = $_POST['username'];
    $newPassword = $_POST['newPassword'];

    // Input validation
    if (empty($username) || empty($newPassword)) {
        echo json_encode(['success' => false, 'message' => 'Username and password cannot be empty']);
        exit;
    }

    $salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
    $hashedPassword = crypt($newPassword, $salt);

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
            echo json_encode(['success' => false, 'message' => 'Failed to update password']);
        }
        
        mysqli_stmt_close($stmt);
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
    }

    mysqli_close($con);
?>