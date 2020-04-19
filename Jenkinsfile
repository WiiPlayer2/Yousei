pipeline {
    agent {
        label 'docker'
    }

    stages {
        stage('Build') {
            steps {
                sh 'docker build -t registry.home.dark-link.info/yousei:latest -f ./Yousei/Dockerfile .'
            }
        }

        stage('Publish') {
            steps {
                sh 'docker image push registry.home.dark-link.info/yousei:latest'
            }
        }
    }
}
