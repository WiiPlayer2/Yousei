pipeline {
    agent {
        label 'docker'
    }

    environment {
        CLEAN_GIT_BRANCH = "${env.GIT_BRANCH.replaceAll('/', '_')}"
    }

    stages {
        stage('Build') {
            steps {
                sh '''#!/bin/bash -xe
                    docker build -t registry.dark-link.info/yousei:${CLEAN_GIT_BRANCH} -f ./YouseiReloaded/Dockerfile .
                '''
            }
        }

        // stage('Publish') {
        //     steps {
        //         sh 'docker image push registry.dark-link.info/yousei:$GIT_BRANCH'
        //     }
        // }
    }
}
