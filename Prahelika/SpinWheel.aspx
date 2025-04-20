<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Spin the Wheel</title>
    <style>
        /* Style for the Spin Wheel */
        #spin-wheel {
            width: 300px;
            height: 300px;
            border-radius: 50%;
            border: 5px solid #000;
            position: relative;
            margin: 0 auto;
            display: flex;
            justify-content: center;
            align-items: center;
            transition: transform 4s ease-out; /* Add transition for smooth spinning */
        }

        /* Options on the wheel */
        .wheel-option {
            position: absolute;
            width: 50%;
            height: 50%;
            background-color: lightgray;
            text-align: center;
            line-height: 50px;
            font-size: 16px;
            border-radius: 50%;
            transform-origin: 100% 100%; /* Rotate around the center */
        }

        /* Positioning each option on the wheel */
        #option1 { transform: rotate(0deg) translateX(100px); }
        #option2 { transform: rotate(72deg) translateX(100px); }
        #option3 { transform: rotate(144deg) translateX(100px); }
        #option4 { transform: rotate(216deg) translateX(100px); }
        #option5 { transform: rotate(288deg) translateX(100px); }

        /* Arrow Styles */
        .arrow {
            width: 30px;
            height: 30px;
            background-color: red;
            position: absolute;
            top: 10px;
            left: 50%;
            margin-left: -15px;
            z-index: 10;
            border-radius: 50%;
        }

        /* Button Styles */
        #spin-btn {
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #4CAF50;
            color: white;
            border: none;
            cursor: pointer;
        }

    </style>
</head>
<body>

    <h2>Spin the Wheel</h2>

    <!-- Spin Wheel -->
    <div id="spin-wheel">
        <div id="option1" class="wheel-option">1. Donate Ramayana</div>
        <div id="option2" class="wheel-option">2. You Won Ramayana</div>
        <div id="option3" class="wheel-option">3. Donate Siva Purana</div>
        <div id="option4" class="wheel-option">4. Donate Devi Bhagavatam</div>
        <div id="option5" class="wheel-option">5. Start YouTube Channel</div>

        <!-- Arrow indicating the selected option -->
        <div class="arrow"></div>
    </div>

    <!-- Button to trigger the spin -->
    <button id="spin-btn" onclick="spinWheel()">Spin the Wheel</button>

    <script>
        function spinWheel() {
            // Randomly select an option
            const spinResult = Math.floor(Math.random() * 5) + 1;

            // Calculate rotation based on selected option
            const angle = spinResult * 72; // Each option is 72 degrees apart (360 / 5)

            // Apply the spin animation with smooth transition
            const wheel = document.getElementById("spin-wheel");
            wheel.style.transition = "transform 4s ease-out"; // Smooth transition
            wheel.style.transform = `rotate(${3600 + angle}deg)`; // 3600 deg (10 full spins) + result angle

            // Disable the button after spinning
            document.getElementById("spin-btn").disabled = true;

            // After the spin ends, re-enable the button
            setTimeout(function () {
                document.getElementById("spin-btn").disabled = false;
            }, 4000); // 4000ms equals to 4 seconds of animation time
        }
    </script>

</body>
</html>
