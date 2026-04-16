<?php
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
    header('Access-Control-Allow-Headers: Content-Type');

    if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
        http_response_code(204);
        exit;
    }

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

    // Split question text into lines at sentence boundaries so authors
    // don't need to add any markers in the CSV themselves.
    function formatQuestion(string $text): string {
        // Strip surrounding literal quote characters added by triple-quote CSV convention
        $text = trim($text, '"');
        // Normalize line endings from multi-line CSV cells
        $text = str_replace("\r\n", "\n", $text);
        $text = str_replace("\r", "\n", $text);
        return trim($text);
    }

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
        if ($header === false || count($header) < 8) {
            echo json_encode(['success' => false, 'message' => 'Invalid CSV format: expected 8 columns']);
            fclose($handle);
            exit;
        }

        $query = "UPDATE questions SET question = ?, answer1 = ?, answer2 = ?, answer3 = ?, answer4 = ?, explanation = ? WHERE levelID = ? AND questionID = ?";

        $stmt = mysqli_prepare($con, $query);
        if (!$stmt) {
            echo json_encode(['success' => false, 'message' => 'Failed to prepare SQL statement: ' . mysqli_error($con)]);
            fclose($handle);
            exit;
        }

        mysqli_stmt_bind_param($stmt, "ssssssss", $question, $answer1, $answer2, $answer3, $answer4, $explanation, $levelID, $questionID);

        $errors = [];
        $rowNum = 1;
        while (($data = fgetcsv($handle)) !== false) {
            if (count($data) < 8) {
                $errors[] = "Row $rowNum: invalid format (expected 8 columns, got " . count($data) . ")";
                $rowNum++;
                continue;
            }

            $question   = formatQuestion(iconv('Windows-1252', 'UTF-8//IGNORE', $data[0]));
            $answer1    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[1]);
            $answer2    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[2]);
            $answer3    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[3]);
            $answer4    = iconv('Windows-1252', 'UTF-8//IGNORE', $data[4]);
            $explanation   = iconv('Windows-1252', 'UTF-8//IGNORE', $data[5]);
            $levelID = strval($data[6]);
            $questionID = strval($data[7]);

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