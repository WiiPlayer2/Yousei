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

        stage('Build App') {
            when {
                anyOf {
                    changeset 'Yousei/**'
                    changeset 'Yousei.Connectors/**'
                    changeset 'Yousei.Core/**'
                    changeset 'Yousei.Shared/**'
                }
            }
            steps {
                sh 'docker build -t registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH -f ./Yousei/Dockerfile .'
            }
        }

        stage('Build Web') {
            when {
                anyOf {
                    changeset 'Yousei.Shared/**'
                    changeset 'Yousei.Web/**'
                }
            }
            steps {
                sh 'docker build -t registry.dark-link.info/yousei-web:$CLEAN_GIT_BRANCH -f ./Yousei.Web/Dockerfile .'
            }
        }

        stage('Publish App') {
            when {
                allOf {
                    branch pattern: "main|dev", comparator: "REGEXP"
                    anyOf {
                        changeset 'Yousei/**'
                        changeset 'Yousei.Connectors/**'
                        changeset 'Yousei.Core/**'
                        changeset 'Yousei.Shared/**'
                    }
                }
            }
            steps {
                withDockerRegistry([credentialsId: 'vserver-container-registry', url: "https://registry.dark-link.info/"]) {
                    sh 'docker tag registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH registry.dark-link.info/yousei:latest'
                    sh 'docker image push registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH'
                    sh 'docker image push registry.dark-link.info/yousei:latest'
                }
            }
        }

        stage('Publish Web') {
            when {
                allOf {
                    branch pattern: "main|dev", comparator: "REGEXP"
                    anyOf {
                        changeset 'Yousei.Shared/**'
                        changeset 'Yousei.Web/**'
                    }
                }
            }
            steps {
                withDockerRegistry([credentialsId: 'vserver-container-registry', url: "https://registry.dark-link.info/"]) {
                    sh 'docker tag registry.dark-link.info/yousei-web:$CLEAN_GIT_BRANCH registry.dark-link.info/yousei-web:latest'
                    sh 'docker image push registry.dark-link.info/yousei-web:$CLEAN_GIT_BRANCH'
                    sh 'docker image push registry.dark-link.info/yousei-web:latest'
                }
            }
        }
    }
}
