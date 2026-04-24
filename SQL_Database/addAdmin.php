<?php
    $mysqli = new mysqli('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
    if ($mysqli->connect_error) {
        die('Connect Error (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
    }

        // Check if the username is provided in the POST request
    if (isset($_POST['username'])) {
        $username = $_POST['username'];

        // Then insert into users
        $stmt = $mysqli->prepare("INSERT INTO users (username, hash, type) VALUES (?, 'test123', 'admin')");
        $stmt->bind_param('s', $username);
        $stmt->execute();

        // Check if the admin was added successfully
        $stmt = $mysqli->prepare("SELECT id FROM users WHERE username = ?");
        $stmt->bind_param('s', $username);
        $stmt->execute();
        $stmt->store_result();


        if ($stmt->affected_rows > 0) {
            echo json_encode(['success' => true, 'message' => 'Admin added successfully']);
        } else {
            echo json_encode(['success' => false, 'message' => 'Admin not added']);
        }
        $stmt->close();
    } else {
        echo json_encode(['success' => false, 'message' => 'Username not provided']);
    }

    