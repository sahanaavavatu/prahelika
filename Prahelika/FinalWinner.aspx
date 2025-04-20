<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinalWinner.aspx.cs" Inherits="Prahelika.FinalWinner" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Final Winner</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        body {
            margin: 0;
            background-color: #000;
            color: #fff;
            font-family: 'Segoe UI', sans-serif;
            text-align: center;
            overflow: hidden;
        }

        .winner-container {
            margin-top: 100px;
        }

        .winner-photo {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            border: 5px solid #ffc300;
            object-fit: cover;
            box-shadow: 0 0 20px #ffc300;
        }

        .winner-name {
            font-size: 36px;
            color: #ffc300;
            margin-top: 20px;
            font-weight: bold;
        }

        .winner-title {
            font-size: 24px;
            color: #fff;
            margin-top: 10px;
        }

        canvas {
            position: fixed;
            top: 0;
            left: 0;
            z-index: -1;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="winner-container">
            <div class="winner-title">🏆 Final Winner of Prahelika Live Quiz 🏆</div>
            <asp:Image ID="imgWinner" runat="server" CssClass="winner-photo" />
            <div class="winner-name">
                <asp:Label ID="lblWinnerName" runat="server" Text="Winner Name" />
            </div>
        </div>
    </form>

    <!-- 🎆 Fireworks JS -->
    <canvas id="fireworksCanvas"></canvas>

    <!-- 🔊 Applause Audio -->
    <audio id="applauseAudio" autoplay>
        <source src="assets/applause.mp3" type="audio/mpeg" />
    </audio>

    <script>
        // Fireworks JS
        const canvas = document.getElementById('fireworksCanvas');
        const ctx = canvas.getContext('2d');
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;

        let particles = [];

        function createFirework(x, y) {
            for (let i = 0; i < 100; i++) {
                particles.push({
                    x: x,
                    y: y,
                    speed: Math.random() * 5 + 2,
                    angle: Math.random() * 2 * Math.PI,
                    radius: Math.random() * 3 + 2,
                    alpha: 1
                });
            }
        }

        function updateParticles() {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            particles = particles.filter(p => p.alpha > 0.05);
            particles.forEach(p => {
                p.x += p.speed * Math.cos(p.angle);
                p.y += p.speed * Math.sin(p.angle);
                p.alpha -= 0.01;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.radius, 0, 2 * Math.PI);
                ctx.fillStyle = `rgba(255, 195, 0, ${p.alpha})`;
                ctx.fill();
            });
        }

        function loop() {
            updateParticles();
            requestAnimationFrame(loop);
        }

        setInterval(() => {
            createFirework(Math.random() * canvas.width, Math.random() * canvas.height / 2);
        }, 1000);

        loop();
    </script>
</body>
</html>
