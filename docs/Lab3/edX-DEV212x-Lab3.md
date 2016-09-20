# edX DEV212x Intro to DevOps - LAB 3, Continuous Integration with Visual Studio Team Services #
This is the Hands on Lab for module 3 of the Intro to DevOps course.

Once you have completed the videos and other course material for Module 3, you can continue with this lab.

In this lab we have an application called PartsUnlimited. We want to set up
Visual Studio Team Services (VSTS) to be able continuously integrate code into the master
branch of code. This means that whenever code is committed and pushed to the
master branch, we want to ensure that it integrates into our code correctly to
get fast feedback. To do so, we are going to be setting up a Continuous Integration build (CI) that
will allow us to compile and run unit tests on our code every time a commit is
pushed to Visual Studio Team Services.

###Pre-requisites:###

-   Make sure you have completed [LAB 1](../Lab1/EdX212x-Lab1.md) to set up your VSTS account and install Git. 

### Tasks Overview: ###

**1. Import Source Code into your VSTS Account:** In this step, you will connect your own Visual Studio Team Services account, download the PartsUnlimited source code, and then push it to your own Visual Studio Team Services account. There are two approaches to doing this. 1) Use the Git command line, or 2) Use Visual Studio. The Git command line is the cleanest and easiest approach, but it does require some familiarity with the Git command line. 

**2. Create Continuous Integration Build:** In this step, you will create a build definition that will be triggered every time a commit is pushed to your repository in Visual Studio Team Services. 

**3. Test the CI Trigger in Visual Studio Team Services:** In this step, test the Continuous Integration build (CI) build we created by changing code in the Parts Unlimited project with Visual Studio Team Services. 

## 1a: Import Source Code into your VSTS Account with Git Command Line

> Note: Use this to approach to use the Git command line to migrate code from GitHub to VSTS. Make sure that you have installed Git and added it to the path from the [task in Lab 1](https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps/blob/master/docs/Lab1/edX-DEV212x-Lab1.md#task-3---download-and-install-git). If you use this approach, skip section 1b. 

We want to push the application code to your Visual Studio Team Services account in
order to use Build.

**1.** Navigate to [https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps](https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps) and copy the URL to the repo. (These steps all use the HTTPS protocol.)  This step will save you from having to type the full URL later.

![](<media/clone.png>)

**2.** Create a parent **Working Directory** on your local file system. For instance, on Windows OS you can create the following directory:

`C:\Source\Repos`

Open a command line window (one that supports Git such as Git Bash) and change to the directory you created above. If not using Git Bash, ensure that Git is in the path for a command line window by typing in `git`. If it isn't, either use Git Bash (installed by default) instead or follow the last steps from the [task in Lab 1](https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps/blob/master/docs/Lab1/edX-DEV212x-Lab1.md#task-3---download-and-install-git) to add it to the path again with the installation bits. 

Clone the repository with the following command. You can paste in the URL if you copied it in Step 1.  In the example below, the clone will be copied into a directory named `PartsUnlimited`. Feel free to use whatever directory name you like, or leave it blank to use the default directory name:

	git clone https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps.git PartsUnlimited

After a few seconds of downloading, all of the code should now be on your local machine.

Move into the directory that was just created.  In a Windows OS (and assuming you used PartsUnlimited as the directory name), you can use this command:

	cd PartsUnlimited

**3.** Remove the link to GitHub. 

The Git repo you just downloaded currently has a remote called _origin_ that points to the GitHub repo.  Since we won't be using it any longer, we can rename it or delete it altogether. You can either delete or rename the remote.  (You can theoretically leave it called _origin_ and use a different name for the VSTS remote. However, in later labs we're going to assume that your remote pointing to VSTS is called _origin_, so I recommend you either delete or rename the repo.)

To delete the GitHub remote, use:

	git remote remove origin

Otherwise, to rename the GitHub remote, use:

	git remote rename origin github

**4.** Find the URL to access the VSTS Git repo

First, we need to find the URL to empty Git repository in VSTS.  If you remember your account name, and the Team Project name you created, the URL to the default Git repo is easily assembled:

	https://<account>.visualstudio.com\<project>\_git\<project>

Alternatively, you can use a web browser to browse to your account, click into your project, and click the Code tab to get to your default Git repository:

	https://<account>.visualstudio.com

Additionally, at the bottom of the web page, you will see the two commands that we will use to push the existing code to VSTS.

![](<media/findVstsRepoUrl.png>)

**5.** Add the link to VSTS and push your local Git repo

In the local directory from Step 3, use the following command to add VSTS as the Git remote named _origin_. You can either type the URL you found in Step 4, or simply copy the first command from the VSTS web page.

	git remote add origin https://<account>.visualstudio.com\<project>\_git\<project>
Now you can push the code, including history, to VSTS:

	git push -u origin --all	

Congratulations, your code should now be in VSTS!

### 1b: Import Source Code into your VSTS Account with Visual Studio

> Note: Use this to approach to use the Visual Studio to migrate code from GitHub to VSTS. If you use this approach, skip section 1a.  **We recommend going through approach 1a because history isn't migrated over when downloading the zip file from GitHub in approach 1b.**

We want to push the application code to your Visual Studio Team Services account in
order to use Build.

**1.** First, we need to open Team Explorer. Go to your **account home
page**:

	https://<account>.visualstudio.com

**2.** Connect to the VSTS account project (you may have called it EdX in Module 1) using Visual Studio (Note: in this image the Project is called HOL).

![](<media/manage_connections.png>)


![](<media/connect_to_vsts.png>)

**3.** Navigate to [https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps](https://github.com/MicrosoftLearning/edX-DEV212x-Intro-to-DevOps) and download the repository as a zip.

![](<media/download.png>)

**4.** Create a **Working Directory** to the following location:

`C:\Source\Repos\PartsUnlimited`

**5.** Extract the zip file to the working directory that you just created. Note: when extracting be sure and “Unblock” the content or the deployment scripts won’t run

![](<media/extract_parts_unlimited.png>)

**6.** Clone the repo of your team project to the location where you extracted the sample

Set the **Working Directory** to the following location:

`C:\Source\Repos\PartsUnlimited`

![](<media/clone_vsts_repo_vs.png>)

**7.** Click Open and navigate to the Parts Unlimited Project Solution in Solution Explorer

![](<media/open_pu_solution.png>)

**8.** Now we will add the source to the Git repo. Right click on the solution and click **Add to Source Control**.

![](<media/add_solution_to_source_control.png>)

**7.** The Changes windows will appear, add in checkin text and verify the source is ready to be committed. Click on **Commit and Sync**. Committing the changes will only record the edits in the local repository, so we will also sync the commits (pull for any new commits from VSTS, then push our commits to VSTS). 

![](<media/commit_and_sync.png>)

**9.** Once the changes have been committed, click on the **Code** hub at the top of
the page. Verify the source is in the repo.

![](<media/parts_unlimited_vsts.png>)

### 2. Create Continuous Integration Build

A continuous integration build will give us the ability check whether the code
we checked in can compile and will successfully pass any automated tests that we
have created against it.

**1.** Since Visual Studio Team Services is already open in a browser, from the **Code** hub, click on the the **Build** hub at the top of the page.

![](<media/build_hub.png>)

**2.** In the build hub, click on the **New definition** button to create a new build definition.

![](<media/create_new_definition.png>)

**3.** In the "Create new build definition" panel, select **Visual Studio Build** as the template for the definition and then click next.

![](<media/select_vs_template.png>)

**4.** After clicking the **Next** button, select the PartsUnlimited Git repository, keep the default branch to build as master, check the **Continuous Integration** checkbox (to trigger a build upon every commit to master), and keep the Hosted queue as the default agent queue.

![](<media/select_repo_trigger_queue.png>)

> **Note:** For this example, we will be using the [hosted agent](https://www.visualstudio.com/en-us/docs/build/admin/agents/hosted-pool) in Visual Studio Team Services. If you are using on-premises TFS at your organization, you will only be able to use on-premises agents (not hosted).

**5.** In the **Build** tab of the new build definition that you just created, leave the NuGet restore task as it is with the defaults. The NuGet packages will be restored for the solution in this step. 

![](<media/nuget_restore.png>)

**6.** On the **Build solution** task, leave the path to the solution as it is (will search for any solution files in the repository and there is only one). Enter the following information to the **MSBuild** Arguments. These arguments will create a Web Deploy (MSDeploy) package for PartsUnlimited into a single zip file in preparation for deployment in Lab 4 and place the package into the artifact staging directory of the agent. 
    
    /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation=$(Build.ArtifactStagingDirectory)
      
> **Note:** Add all of the **MSBuild** parameters one after the other with spaces between them.

![](<media/add_msbuild_args.png>)

**7.** The rest of the tasks accomplish the following:
	Test Assemblies - Searches for automated test dlls in the compiled code after build and runs the tests locally.
	Publish symbols path - Publishes symbols (if specified) for debugging purposes. 

**8.** Remove the **Copy Files to: $(build.artifactstagingdirectory)** task with the "X" to the right of the task. Then, click the "Add build step..." button above the tasks to add a new task to copy and publish build artifacts. 

![](<media/remove_copy_task.png>)

> **Note:** Since we specify to place the package in the artifact staging directory in the MSBuild argument in step 6, we can specify the artifact staging directory to copy and publish in one step for simplicity. 

**9.** In the task catalog, click on the **Utility** tab and add a **Copy and Publish Build Artifacts** task to add it to the build definition. The order of the task (before or after the "Publish Artifact: drop" task) does not matter as long as it is after the "Publish symbols path" task. 

![](<media/add_copy_publish_build_artifacts.png>)

**10.** In the **Copy Publish Artifacts** task that you just added, set the Copy Root to `$(Build.ArtifactStagingDirectory)` (where the packaged was pushed), set the Contents to `**\*.zip`, set the Artifact Name to "drop", and choose "Server" as the Artifact Type in the dropdown. This will copy any zip files in the folder and publish them as build artifacts. 

![](<media/update_copy_publish_artifact.png>)

**11.** Click on the **Publish artifact: drop** task and change the Path to Publish to be `src/env/PartsUnlimitedEnv/Templates`. Then, rename the Artifact Name to be "ARMTemplates." Since we're already publishing the PartsUnlimited zip file as a build artifact in the previous step, we will use this task instead to publish the Azure Resource Manager (ARM) templates for use in Lab 4.

![](<media/publish_arm_templates.png>)

**12.** Click on the **Save** button to save the build definition and give it a name. 

![](<media/save_build_definition.png>)

### 3. Test the CI Trigger in Visual Studio Team Services

We will now test the **Continuous Integration build (CI)** build we created in *Task 2* by changing code in the Parts Unlimited project with Visual Studio Team Services.

**1.** Click on the **Code** hub and then select your repo, such as "PartsUnlimited". Navigate to **src/src/PartsUnlimitedWebsite/Controllers/HomeController.cs** in the PartsUnlimited project, then click **Edit**.

![](<media/open_home_controller.png>)

**2.** After clicking **Edit**, add in a comment (e.g. `//comment`) to the top of the constuctor of the **HomeController.cs** file. Once complete, click **Save**. The Save button will actually commit the change on the master branch to the Git repo in VSTS and will automatically trigger a build.

![](<media/edit_home_controller.png>)

**3.** Click the **Build** hub and note that a build has queued that was requested by your username. Click on the build number (such as "#20160920.1") to view the build's progress.

![](<media/view_running_build.png>)

**4.** The console will show the log of the running build and should show that the build succeeded. If you click on the Build Number at the left panel, you can view the build summary as well. 

![](<media/view_build_console.png>)

![](<media/view_build_summary.png>)

In this lab, you learned how to add an existing Git repo to a project in Visual Studio, create a Continuous Integration build definition to compile, test, and publish build artifacts, and the end-to-end workflow of committing code to master and automatically triggering a build. 

## Further Reading
1. [Release Management for Visual Studio Team Services](https://msdn.microsoft.com/Library/vs/alm/release/overview-rmpreview)
2. [Cloud Load Testing in Visual Studio Team Services](https://channel9.msdn.com/Events/Visual-Studio/Connect-event-2015/Cloud-Loading-Testing-in-Visual-Studio-Team-Service)

The following are other PartsUnlimited Hands on Labs that you can try in your own time. **These are not required for the EdX course!**:

1. [User Telemetry with Application Insights](https://github.com/Microsoft/PartsUnlimited/blob/hands-on-labs/docs/HOL_PartsUnlimited_HDD-User-Telemetry/HOL_PartsUnlimited_HDD-User-Telemetry.md)
2. [Testing in Production with Azure Websites - PartsUnlimited](https://github.com/Microsoft/PartsUnlimited/blob/hands-on-labs/docs/HOL_HDD_Testing_in_Production/HDD%20Testing%20in%20Production%20with%20Azure%20Websites%20HOL.md)
3. [Application Performance Monitoring - PartsUnlimited](https://github.com/Microsoft/PartsUnlimited/blob/hands-on-labs/docs/HOL_PartsUnlimited_Application_Performance_Monitoring/HOL_PartsUnlimited_Application_Performance_Monitoring.md)
