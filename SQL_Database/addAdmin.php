<?php
header('Content-Type: application/json');

mysqli_report(MYSQLI_REPORT_OFF);

$mysqli = new mysqli('103-89-14-188.cloud-xip.com', 'root', 'GoDentalCougars66@!', 'oral_medicine', 3306);
if ($mysqli->connect_error) {
    error_log('addAdmin.php DB connect error: ' . $mysqli->connect_error);
    http_response_code(500);
    echo json_encode(['success' => false, 'message' => 'Database connection failed']);
    exit;
}

if (!isset($_POST['username'])) {
    echo json_encode(['success' => false, 'message' => 'Username not provided']);
    $mysqli->close();
    exit;
}

$username = trim($_POST['username']);
if ($username === '') {
    echo json_encode(['success' => false, 'message' => 'Username cannot be empty']);
    $mysqli->close();
    exit;
}

// users.username is VARCHAR(36); reject values that exceed schema length.
if (strlen($username) > 36) {
    echo json_encode(['success' => false, 'message' => 'Email is too long (maximum 36 characters)']);
    $mysqli->close();
    exit;
}
        
$defaultPassword = 'test123';
$salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
$hash = crypt($defaultPassword, $salt);

if ($hash === false || strlen($hash) < 10) {
    error_log('addAdmin.php hash generation failed for username: ' . $username);
    http_response_code(500);
    echo json_encode(['success' => false, 'message' => 'Failed to generate password hash']);
    $mysqli->close();
    exit;
}

$insert = $mysqli->prepare("INSERT INTO users (username, hash, salt, type) VALUES (?, ?, ?, 'admin')");
if (!$insert) {
    error_log('addAdmin.php prepare error: ' . $mysqli->error);
    http_response_code(500);
    echo json_encode(['success' => false, 'message' => 'Failed to prepare insert statement']);
    $mysqli->close();
    exit;
}

$insert->bind_param('sss', $username, $hash, $salt);
$executed = $insert->execute();

if (!$executed) {
    if ($mysqli->errno === 1062) {
        echo json_encode(['success' => false, 'message' => 'That email is already registered']);
    } else {
        error_log('addAdmin.php execute error [' . $mysqli->errno . ']: ' . $mysqli->error);
        http_response_code(500);
        echo json_encode(['success' => false, 'message' => 'Failed to add admin']);
    }
    $insert->close();
    $mysqli->close();
    exit;
}

$insert->close();
echo json_encode(['success' => true, 'message' => 'Admin added successfully']);
$mysqli->close();
?>

