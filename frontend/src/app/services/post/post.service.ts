import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs'
import { Post } from 'src/app/models/post';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Allow': '*',
  })
}
@Injectable({
  providedIn: 'root'
})
export class PostService {

  private apiUrl = 'https://localhost:7084/api/Recipe'

  public userPosts$ :Observable<Post[]>;
  constructor(private http: HttpClient) { }

  getRecipeById(id: string): Observable<Post> {
    const url = `${this.apiUrl}/GetRecipe/${id}`;
    return this.http.get<Post>(url, httpOptions);
  }

  createNewRecipe(recipeInfo){
    const url = `${this.apiUrl}/AddRecipe`;
    const username = localStorage.getItem('username');
    
    const token = localStorage.getItem('token');
    httpOptions.headers.set('Authorization', token);

    const newRecipe  =  {
      IsDeleted: 0,
      UserRef: username,
      Title:  recipeInfo.title,
      Description: recipeInfo.description,
      Ingredients: [...recipeInfo.ingredients],
      Comments: []
    };
    return this.http.post(url, newRecipe, {headers:{
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Allow': '*',
      'Authorization': token,
    }, responseType: "text"});
  }

  getRecipesByUser(){
    const url = `${this.apiUrl}/GetAllRecipes`;
    const token = localStorage.getItem('token');

    this.userPosts$ = this.http.get<Post[]>(url,{headers:{
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Allow': '*',
      'Authorization': token,
    }});
  }

  getAllRecipes() {
    const url = `${this.apiUrl}/GetAllRecipes`
    return this.http.get<Post[]>(url, httpOptions)
  }
}
