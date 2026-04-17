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

    $query = "SELECT id from user where username = ?";
    $stmt = mysqli_prepare($con, $query);
    mysqli_stmt_bind_param($stmt, "s", $username);
    mysqli_stmt_execute($stmt);
    mysqli_stmt_store_result($stmt);
    if (mysqli_stmt_num_rows($stmt) == 0) {
        echo json_encode(value: ['success' => false, 'message' => 'User not found']);
        mysqli_stmt_close($stmt);
        mysqli_close($con);
        exit();
    }
    else {
        $id = mysqli_stmt_get_result($stmt)->fetch_assoc()['id'];
    }
    mysqli_stmt_close($stmt);

    $query = "UPDATE user SET $column = ? WHERE id = ?";
    $stmt = mysqli_prepare($con, $query);
    mysqli_stmt_bind_param($stmt, "ii", $score, $id);
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

