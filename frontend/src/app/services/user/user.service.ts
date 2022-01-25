import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { User } from 'src/app/models/user';

const baseUrl = 'https://localhost:7084/api' ;
const service = {
  'Auth': "Auth",
  'User': "User",
};

interface logInSessionToken{
  token : string;
}
@Injectable({
  providedIn: 'root'
})
export class UserService {

  private userSubject: BehaviorSubject<User>;
  public user: Observable<User>;
  public loggedIn : boolean = false;
  constructor(
    private router: Router,
    private http: HttpClient,
  ) {
    this.userSubject = new BehaviorSubject<User>(
      JSON.parse(localStorage.getItem('currentUser'))
    );
    this.user = this.userSubject.asObservable();
  }

  public get userValue(): User {
    return this.userSubject.value;
  }

   login(username: string, password: string) {
    console.log("logging in");
    
    const post =  this.http
    .post('https://localhost:7084/api/Auth/login', {
       "Username":username,
      "Password": password,
    },{headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }});;
    
    post.toPromise().then((data : logInSessionToken) => {
      console.log("ejta",data);
      const token = data.token;
      console.log("tokenche",token);
      localStorage.setItem('token', token );
      localStorage.setItem('username', username);
      this.setUserObservable(token);
      this.loggedIn = true;
      this.router.navigate(['']);
    })
    return  post;
      
  }
  setUserObservable(token){
    const username = localStorage.getItem('username');
    const response = this.http
    .get<User>('https://localhost:7084/api/User/GetUser/'+username,{headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization' : token
    }});
    console.log("resposne is user", response);
    this.user = response;
  }

   logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    this.userSubject.next(null);
    this.loggedIn = false;
    this.router.navigate(['/account/login']);
   }

  register(user: User) {
    console.log("registering",user.username,user.password,user);
    return this.http.post("https://localhost:7084/api/User/AddUser", user, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }});
  }

  getUserPosts(username : string){
    //http request here

    return ["post 1", "post 2", "post 3", "post 4","post 5","post 6","post 7","post 8","post 9","post 10",];
  }

  getFollowedUsers(username : string){
    //http request here

    return ["getFollowedUsers 1", "getFollowedUsers 2", "getFollowedUsers 3"];
  }
  
  getUserFavouritePosts(username : string){
    //http request here

    return ["favourite post 1", "favourite post 2", "favourite post 3"];
  }

  subscribeToAuthor(username: string, userToFollow: string){
    const url = `https://localhost:7084/api/User/FollowUser/${username}/${userToFollow}`
    return this.http.get(url, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': localStorage.getItem("token")
    }})
  }

  unsubscribeFromAuthor(username: string, userToFollow: string) {
    const url = `https://localhost:7084/api/User/UnfollowUser/${username}/${userToFollow}`
    return this.http.get(url, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': localStorage.getItem("token")
    }})
  }
 
  getUser(username: string) {
    const url = `https://localhost:7084/api/User/GetUser/${username}`
    return this.http.get<User>(url, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': localStorage.getItem("token")
    }})
  }

  addToFavorites(username: string, recipeId: string) {
    const url = `https://localhost:7084/api/User/AddRecipeToFavorites/${username}/${recipeId}`
    return this.http.get(url, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': localStorage.getItem("token")
    }})
  }

  removeFromFavorites(username: string, recipeId: string) {
    const url = `https://localhost:7084/api/User/RemoveRecipeToFavorites/${username}/${recipeId}`
    return this.http.get(url, {headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Authorization': localStorage.getItem("token")
    }})
  }
}
