import { Component, OnInit, Input } from '@angular/core';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post/post.service';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  @Input() post: Post = new Post(1, "Miladin", "Gulas", "Lako se sprema", ["ulje", "jaja", "krompir", "meso"] ,[new Comment("Kika", "Dobro"), new Comment("Micko", "Super supica")])
  


  constructor(private postService: PostService) { }

  ngOnInit(): void {
    // this.postService.getRecipeById(2).subscribe((resultPost) => {
    //    this.post = resultPost;
    //   console.log("Ovo ovde", resultPost, "Ovo ovde 2",this.post);
    // })
  }

  loggedIn(): boolean{
    return localStorage.getItem("username") ? true : false
  }
}
