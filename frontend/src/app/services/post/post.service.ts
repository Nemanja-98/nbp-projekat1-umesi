import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Observable } from 'rxjs'
import { Post } from 'src/app/models/post';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*',
    'Allow': '*'
  })
}
@Injectable({
  providedIn: 'root'
})
export class PostService {

  private apiUrl = 'https://localhost:7084/api/Recipe/GetRecipe/'

  constructor(private http: HttpClient) { }

  getRecipeById(id: number): Observable<Post> {
    const url = `${this.apiUrl}${id}`
    return this.http.get<Post>(url, httpOptions);
  }

}
