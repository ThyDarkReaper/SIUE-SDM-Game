<?php

    header(header: 'Content-Type: application/json');

    if(!isset($_POST['username']) || !isset($_POST['score']) || !isset($_POST['level'])) {
        echo json_encode(value: ['success' => false, 'message' => 'Missing required fields']);
        exit();
    }

    $con = mysqli_connect(hostname: '103-89-14-188.cloud-xip.com', username: 'root', password: 'GoDentalCougars66@!', database: 'oral_medicine', port: 3306);
    if (!$con) {
        echo json_encode(value: ['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit();
    }

    $username = $_POST['username'];
    $score = $_POST['score'];
    $level = $_POST['level'];

    if (empty($username) || !is_numeric($score) || !is_string($level)) {
        echo json_encode(value: ['success' => false, 'message' => 'Invalid input data', 'username type' => gettype($username), 'score type' => gettype($score), 'level type' => gettype($level)]);
        exit();
    }

    $column = $level;
    $query = "UPDATE student SET $column = ? WHERE username = ?";
    $stmt = mysqli_prepare($con, $query);
    mysqli_stmt_bind_param($stmt, "is", $score, $username);
    $result = mysqli_stmt_execute($stmt);
    if ($result) {
        $affectedRows = mysqli_stmt_affected_rows($stmt);
        if ($affectedRows == 1) {
            echo json_encode(value: ['success' => true, 'message' => "Level $level score updated successfully"]);
        } else {
            echo json_encode(value: ['success' => false, 'message' => 'User not found or no changes made']);
        }
    } else {
        echo json_encode(value: ['success' => false, 'message' => 'Failed to execute query']);
    }
    mysqli_stmt_close($stmt);
    mysqli_close($con);

