<?php
    $mysqli = new mysqli('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
    if ($mysqli->connect_error) {
        die('Connect Error (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
    }

    if (isset($_POST['username'])) {
        $username = $_POST['username'];

        // Then insert into users
        $stmt = $mysqli->prepare("INSERT INTO users (username, password) VALUES (?, 'test123')");
        $stmt->bind_param('s', $username);
        $stmt->execute();

        // Insert into students (references users.id via userid foreign key)
        $stmt = $mysqli->prepare("INSERT INTO students (userid) VALUES ((SELECT id FROM users WHERE username = ?))");
        $stmt->bind_param('s', $username);
        $stmt->execute();
        $stmt->close();

        // Check if the player was added successfully
        $stmt = $mysqli->prepare("SELECT id FROM users WHERE username = ?");
        $stmt->bind_param('s', $username);
        $stmt->execute();
        $stmt->store_result();


        if ($stmt->affected_rows > 0) {
            echo json_encode(['success' => true, 'message' => 'Player added successfully']);
        } else {
            echo json_encode(['success' => false, 'message' => 'Player not added']);
        }
        $stmt->close();
    } else {
        echo json_encode(['success' => false, 'message' => 'Username not provided']);
    }

    