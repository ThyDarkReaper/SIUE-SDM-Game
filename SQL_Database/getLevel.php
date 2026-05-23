<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

if (!isset($_GET['level'])) {
    echo json_encode(['success' => false, 'message' => 'Missing required fields']);
    exit;
}

$con = mysqli_connect('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if (!$con) {
    echo json_encode(['success' => false, 'message' => 'Database connection failed: ' . mysqli_connect_error()]);
    exit;
}

$level = $_GET['level'];

if (empty($level)) {
    echo json_encode(['success' => false, 'message' => 'Level cannot be empty']);
    exit;
}

$query = "SELECT question, answer1, answer2, answer3, answer4, explanation FROM questions WHERE levelID = ? ORDER BY questionID ASC";
$stmt = mysqli_prepare($con, $query);
if ($stmt) {
    mysqli_stmt_bind_param($stmt, "s", $level);
    $result = mysqli_stmt_execute($stmt);
    
    if ($result) {
        $received = mysqli_stmt_get_result($stmt);
        $questions = [];
        while ($received && $row = mysqli_fetch_assoc($received)) {
            $questions[] = [
                'question'    => $row['question'],
                'answer1'     => $row['answer1'],
                'answer2'     => $row['answer2'],
                'answer3'     => $row['answer3'],
                'answer4'     => $row['answer4'],
                'explanation' => $row['explanation']
            ];
        }
        if (count($questions) > 0) {
            echo json_encode(['success' => true, 'level' => $level, 'questions' => $questions]);
        } else {
            echo json_encode(['success' => false, 'message' => 'No questions found for this level']);
        }
    } else {
        echo json_encode(['success' => false, 'message' => 'Failed to execute query']);
    }
    
    mysqli_stmt_close($stmt);
} else {
    echo json_encode(['success' => false, 'message' => 'Failed to prepare statement']);
}

mysqli_close($con);