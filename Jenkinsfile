pipeline {
    agent {
        label 'docker'
    }

    environment {
        CLEAN_GIT_BRANCH = "${env.GIT_BRANCH.replaceAll('/', '_')}"
    }

    stages {
        stage('Check Integrity') {
            when { changeRequest() }
            steps {
                script {
                    if(env.CHANGE_TARGET == 'main' && !(env.CHANGE_BRANCH ==~ /(release|hotfix)\/.+/)) {
                        error('Only release and hotifx branches are allowed.')
                    }
                    if(env.CHANGE_TARGET == 'dev' && !(env.CHANGE_BRANCH ==~ /(feature|bug|hotfix)\/.*/)) {
                        error('Only feature, bug and hotfix branches are allowed.')
                    }
                }
            }
        }

        stage('Build') {
            steps {
                sh '''#!/bin/bash -xe
                    docker build -t registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH -f ./YouseiReloaded/Dockerfile .
                '''
            }
        }

        stage('Publish') {
            when {
                branch pattern: "main|dev", comparator: "REGEXP"
            }
            steps {
                withDockerRegistry([credentialsId: 'vserver-container-registry', url: "https://registry.dark-link.info/"]) {
                    sh 'docker image push registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH'
                }
            }
        }
    }
}
