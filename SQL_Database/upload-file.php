<?php

    // Catch fatal errors and return them as JSON instead of an empty 500
    ob_start();
    register_shutdown_function(function () {
        $error = error_get_last();
        if ($error && in_array($error['type'], [E_ERROR, E_PARSE, E_CORE_ERROR, E_COMPILE_ERROR])) {
            ob_clean();
            http_response_code(500);
            echo json_encode([
                'success' => false,
                'message' => 'PHP Fatal Error: ' . $error['message'] . ' in ' . basename($error['file']) . ' on line ' . $error['line']
            ]);
        }
        ob_end_flush();
    });

    header('Content-Type: application/json');

    $con = mysqli_connect('103.89.14.188', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
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
        // Skip the header row
        $header = fgetcsv($handle);
        if ($header === false || count($header) < 7) {
            echo json_encode(['success' => false, 'message' => 'Invalid CSV format: expected 7 columns']);
            fclose($handle);
            exit;
        }

        $query = "UPDATE questions SET question = ?, answer1 = ?, answer2 = ?, answer3 = ?, answer4 = ?, explanation = ? WHERE questionID = ?";

        $stmt = mysqli_prepare($con, $query);
        if (!$stmt) {
            echo json_encode(['success' => false, 'message' => 'Failed to prepare SQL statement: ' . mysqli_error($con)]);
            fclose($handle);
            exit;
        }

        mysqli_stmt_bind_param($stmt, "sssssss", $question, $answer1, $answer2, $answer3, $answer4, $explanation, $questionID);

        $errors = [];
        $rowNum = 1;
        while (($data = fgetcsv($handle)) !== false) {
            if (count($data) < 7) {
                $errors[] = "Row $rowNum: invalid format (expected 7 columns, got " . count($data) . ")";
                $rowNum++;
                continue;
            }

            $question   = iconv('Windows-1252', 'UTF-8//IGNORE', $data[0]);
            $answer1    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[1]);
            $answer2    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[2]);
            $answer3    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[3]);
            $answer4    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[4]);
            $explanation   = iconv('Windows-1252', 'UTF-8//IGNORE', $data[5]);
            $questionID = strval($data[6]);

            if (!mysqli_stmt_execute($stmt)) {
                $errors[] = "Row $rowNum: " . mysqli_stmt_error($stmt);
            }
            $rowNum++;
        }

        mysqli_stmt_close($stmt);
        mysqli_close($con);
        fclose($handle);

        if (!empty($errors)) {
            echo json_encode(['success' => false, 'message' => 'Some rows failed: ' . implode('; ', $errors)]);
        } else {
            echo json_encode(['success' => true, 'message' => 'File uploaded and processed successfully']);
        }
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to open uploaded file']);
    }