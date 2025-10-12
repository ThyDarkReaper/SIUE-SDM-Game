<?php
session_start();
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    $username  = $_POST["username"];
    $password = $_POST["password"];
    try {
        require_once "dbc.php";
        $query = "SELECT * FROM users where username = :username AND password = :password";
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(":username", $username);
        $stmt->bindParam(":password", $password);
        $stmt->execute();
        
        $user = $stmt->fetch(PDO::FETCH_ASSOC);
        if ($user) {
            $_SESSION['username'] = $username;
            $_SESSION['loggedin'] = true;
            header("Location: welcome.php");
        } else {
            echo "Invalid username or password.";
        }
    }
}