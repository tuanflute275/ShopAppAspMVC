pipeline {
    agent any
    
    environment {
        DOCKER_CREDENTIALS = credentials('dockerhub')
        SQL_SERVER_LOGIN = credentials('sql-server-login')
    }

    tools {
        // Cài d?t .NET SDK dã du?c c?u hình trong Jenkins
        dotnet 'dotnet-sdk'
    }

    stages {
        stage('Restore Dependencies') {
            steps {
                echo 'Restoring .NET dependencies...'
                sh 'dotnet restore'
            }
        }

        stage('Build Application') {
            steps {
                echo 'Building the ASP.NET Core application...'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish Application') {
            steps {
                echo 'Publishing the application...'
                sh 'dotnet publish --configuration Release --output ./publish'
            }
        }

        stage('Create Docker Image') {
            steps {
                echo 'Creating Docker image for the ASP.NET Core app...'
                sh 'docker build -t ${DOCKER_CREDENTIALS_USR}/aspnetapp .'
            }
        }

        stage('Push Docker Image') {
            steps {
                echo 'Pushing Docker image to Docker Hub...'
                withDockerRegistry(credentialsId: 'dockerhub', url: '') {
                    sh 'docker push ${DOCKER_CREDENTIALS_USR}/aspnetapp'
                }
            }
        }

        stage('Deploy SQL Server') {
            steps {
                echo 'Deploying SQL Server...'
                sh 'docker pull mcr.microsoft.com/mssql/server:2019-latest'
                sh '''
                   docker run --name sql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=${SQL_SERVER_LOGIN_PSW}" \
                   -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
                '''
                // Ch? SQL Server kh?i d?ng
                sh 'sleep 30'
            }
        }

        stage('Deploy ASP.NET Core App') {
            steps {
                echo 'Deploying ASP.NET Core application...'
                sh 'docker network create dev-network || echo "Network already exists"'
                sh '''
                    docker run --name aspnetapp --rm -d -p 8080:80 --network dev-network \
                    -e "ConnectionStrings__DefaultConnection=Server=sql-server;Database=MyAppDb;User Id=sa;Password=${SQL_SERVER_LOGIN_PSW}" \
                    ${DOCKER_CREDENTIALS_USR}/aspnetapp
                '''
            }
        }
    }

    post {
        always {
            echo 'Cleaning up workspace...'
            cleanWs()
        }
    }
}
