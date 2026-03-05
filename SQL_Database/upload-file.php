<?php

    header(header: 'Content-Type: application/json');

    $con = mysqli_connect('103.89.14.188', 'root', 'GoDentalCougars66@!', 'oral_medicine', port: 3306);
    if (!$con) {
        echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
        exit;
    }

    if (!isset($_FILES['file']) || $_FILES['file']['error'] !== UPLOAD_ERR_OK) {
        echo json_encode(['success' => false, 'message' => 'No file uploaded or upload error']);
        exit;
    }

    $fileTmpPath = $_FILES['file']['tmp_name'];
    $fileName = $_FILES['file']['name'];

    if (pathinfo($fileName, PATHINFO_EXTENSION) !== 'csv') {
        echo json_encode(['success' => false, 'message' => 'Invalid file type. Only CSV files are allowed']);
        exit;
    }

    if (($handle = fopen($fileTmpPath, 'r')) !== false) {
        $header = fgetcsv($handle);
        if ($header === false || count($header) < 2) {
            echo json_encode(['success' => false, 'message' => 'Invalid CSV format']);
            fclose($handle);
            exit;
        }

        $question = $header[0];
        $answer1 = $header[1];
        $answer2 = $header[2];
        $answer3 = $header[3];
        $answer4 = $header[4];
        $feedback = $header[5];
        $questionID = $header[6];

        $query = "INSERT INTO questions (question, answer1, answer2, answer3, answer4, feedback) VALUES (?, ?, ?, ?, ?, ?) where questionID = ?";

        $stmt = mysqli_prepare($con, $query);
        if (!$stmt) {
            echo json_encode(['success' => false, 'message' => 'Failed to prepare SQL statement: ' . mysqli_error($con)]);
            fclose($handle);
            exit;
        }

        mysqli_stmt_bind_param($stmt, "sssssss", $question, $answer1, $answer2, $answer3, $answer4, $feedback, $questionID);

        while (($data = fgetcsv($handle)) !== false) {
            if (count($data) < 7) {
                echo json_encode(['success' => false, 'message' => 'Invalid CSV format in data row']);
                continue;
            }

            $question = $data[0];
            $answer1 = $data[1];
            $answer2 = $data[2];
            $answer3 = $data[3];
            $answer4 = $data[4];
            $feedback = $data[5];
            $questionID = $data[6];

            if (!mysqli_stmt_execute($stmt)) {
                echo json_encode(['success' => false, 'message' => 'Failed to execute SQL statement: ' . mysqli_stmt_error($stmt)]);
            }
        }    
        mysqli_stmt_close($stmt);
        mysqli_close($con);
        fclose($handle);
        echo json_encode(['success' => true, 'message' => 'File uploaded and processed successfully']);
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to open uploaded file']);
    }