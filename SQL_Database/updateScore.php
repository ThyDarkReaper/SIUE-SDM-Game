<?php

    header(header: 'Content-Type: application/json');

    if(!isset($_POST['username']) || !isset($_POST['score']) || !isset($_POST['level'])) {
        echo json_encode(value: ['success' => false, 'message' => 'Missing required fields']);
        exit();
    }

    $con = mysqli_connect(hostname: 'localhost', username: 'root', password: 'root', database: 'oral_medicine', port: 3306);
    if (!$con) {
        echo json_encode(value: ['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit();
    }

    $username = $_POST['username'];
    $score = $_POST['score'];
    $level = $_POST['level'];

    if (empty($username) || !is_numeric($score) || !is_numeric($level)) {
        echo json_encode(value: ['success' => false, 'message' => 'Invalid input data']);
        exit();
    }

    $query = "UPDATE users SET score = ?, level = ? where username = ?";
    $stmt = mysqli_prepare($con, $query);
    mysqli_stmt_bind_param($stmt, "iis", $score, $level, $username);
    $result = mysqli_stmt_execute($stmt);
    if ($result) {
        $affectedRows = mysqli_stmt_affected_rows($stmt);
        if ($affectedRows == 1) {
            echo json_encode(value: ['success' => true, 'message' => 'Score and level updated successfully']);
        } else {
            echo json_encode(value: ['success' => false, 'message' => 'User not found or no changes made']);

        }
    } else {
        echo json_encode(value: ['success' => false, 'message' => 'Failed to execute query']);
    }
    mysqli_stmt_close($stmt);
    mysqli_close($con);

