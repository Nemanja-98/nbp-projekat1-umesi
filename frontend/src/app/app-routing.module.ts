import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AboutUsComponent } from './components/about-us/about-us.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { PostComponent } from './components/post/post.component';
import { PostsComponent } from './components/posts/posts.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RegisterComponent } from './components/register/register.component';


const routes: Routes = [
  
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'registration', component: RegisterComponent },
  { path: 'post/:id', component: PostComponent },
  { path: 'posts', component: PostsComponent },

  { path: 'about', component: AboutUsComponent },
  // to do add profile page with fav, myPosts, MyComments lists.
  { path: 'profile', component: ProfileComponent },
  
  // to do add blog page (container page for posts)
  // { path: 'blog', component: BlogComponent },
  
  // to do add id kao parametar rute post-a
  { path: 'post', component: PostComponent },
  { path: '**', component: HomeComponent },
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
