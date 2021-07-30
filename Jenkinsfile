def built_app = false;
def built_web = false;

pipeline {
    agent {
        label 'docker'
    }

    environment {
        CLEAN_GIT_BRANCH = "${env.GIT_BRANCH.replaceAll('/', '_')}"
        DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
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
            parallel {
                stage('Build App') {
                    when {
                        anyOf {
                            environment name: 'BUILD_NUMBER', value: '1'
                            changeset 'Yousei/**'
                            changeset 'Yousei.Connectors/**'
                            changeset 'Yousei.Core/**'
                            changeset 'Yousei.Shared/**'
                            changeset 'Yousei.SourceGen/**'
                            expression { currentBuild.previousBuild == currentBuild.previousFailedBuild }
                        }
                    }
                    steps {
                        sh 'docker build -t registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH -f ./Yousei/Dockerfile .'
                        script {
                            built_app = true;
                        }
                    }
                }

                stage('Build Web') {
                    when {
                        anyOf {
                            environment name: 'BUILD_NUMBER', value: '1'
                            changeset 'Yousei.Core/**'
                            changeset 'Yousei.Shared/**'
                            changeset 'Yousei.SourceGen/**'
                            changeset 'Yousei.Web/**'
                            expression { currentBuild.previousBuild == currentBuild.previousFailedBuild }
                        }
                    }
                    steps {
                        sh 'docker build -t registry.dark-link.info/yousei-web:$CLEAN_GIT_BRANCH -f ./Yousei.Web/Dockerfile .'
                        script {
                            built_web = true;
                        }
                    }
                }
            }
        }

        stage('Test') {
            steps {
                sh "find . -name '*.trx' -delete"
                sh "find . -name 'coverage.cobertura.xml' -delete"
                script {
                    docker.image('mcr.microsoft.com/dotnet/sdk:5.0').inside {
                        sh 'dotnet test ./Yousei.sln --configuration Release --collect:"XPlat Code Coverage" --logger "console;verbosity=detailed" --logger trx'
                    }
                }
            }
            post {
                always {
                    mstest testResultsFile:"**/*.trx", keepLongStdio: true
                    cobertura autoUpdateHealth: false,
                        autoUpdateStability: false,
                        coberturaReportFile: '**/coverage.cobertura.xml',
                        enableNewApi: true,
                        failUnhealthy: true,
                        conditionalCoverageTargets: '70, 50, 0',
                        lineCoverageTargets: '80, 60, 0',
                        methodCoverageTargets: '80, 60, 0',
                        zoomCoverageChart: false
                }
            }
        }

        stage('Publish') {
            when { branch pattern: "main|dev", comparator: "REGEXP" }
            parallel {
                stage('Publish App') {
                    when { expression { built_app } }
                    steps {
                        withDockerRegistry([credentialsId: 'vserver-container-registry', url: "https://registry.dark-link.info/"]) {
                            sh 'docker tag registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH registry.dark-link.info/yousei:latest'
                            sh 'docker image push registry.dark-link.info/yousei:$CLEAN_GIT_BRANCH'
                            sh 'docker image push registry.dark-link.info/yousei:latest'
                        }
                    }
                }

                stage('Publish Web') {
                    when { expression { built_web } }
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
    }
}
