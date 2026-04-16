<?php
    $mysqli = new mysqli('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
    if ($mysqli->connect_error) {
        die('Connect Error (' . $mysqli->connect_errno . ') ' . $mysqli->connect_error);
    }

    if (isset($_POST['username'])) {
        $username = $_POST['username'];

        // Delete from students first (references users.id via userid foreign key)
        $stmt = $mysqli->prepare("DELETE FROM students WHERE userid = (SELECT id FROM users WHERE username = ?)");
        $stmt->bind_param('s', $username);
        $stmt->execute();
        $stmt->close();

        // Then delete from users
        $stmt = $mysqli->prepare("DELETE FROM users WHERE username = ?");
        $stmt->bind_param('s', $username);
        $stmt->execute();

        if ($stmt->affected_rows > 0) {
            echo json_encode(['success' => true, 'message' => 'Player deleted successfully']);
        } else {
            echo json_encode(['success' => false, 'message' => 'Player not found']);
        }
        $stmt->close();
    } else {
        echo json_encode(['success' => false, 'message' => 'Username not provided']);
    }

    