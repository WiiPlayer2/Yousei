pipeline {
    agent {
        label 'docker'
    }

    stages {
        stage('Build') {
            steps {
                sh 'docker build -t registry.dark-link.info/yousei:${GIT_BRANCH//\\//_} -f ./YouseiReloaded/Dockerfile .'
            }
        }

        // stage('Publish') {
        //     steps {
        //         sh 'docker image push registry.dark-link.info/yousei:$GIT_BRANCH'
        //     }
        // }
    }
}
