import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Comment } from 'src/app/models/comment';

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
export class CommentService {

  private apiUrl = 'https://localhost:7084/api/Comment/AddComment/'

  constructor(private http: HttpClient) { }

  postComment(comment: Comment, recipeId: string) {
    const url = `${this.apiUrl}${recipeId}`
    return this.http.post<Comment>(url, {
      "userRef": comment.userRef,
      "description": comment.description,
      "isDeleted": 0
    } ,httpOptions);
  }
}