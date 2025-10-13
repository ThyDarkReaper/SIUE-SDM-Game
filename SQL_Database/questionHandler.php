<?php
    if ($_SERVER["REQUEST_METHOD"] == "GET") {
        try {
            require_once "dbc.php";
            $query = "SELECT * FROM questions";
            $stmt = $pdo->prepare($query);
            $stmt->execute();
            
            $questions = $stmt->fetchAll(PDO::FETCH_ASSOC);
            echo json_encode($questions);
        } 
        catch (PDOException $e) {
            echo "Connection failed: " . $e->getMessage();
        }
    }