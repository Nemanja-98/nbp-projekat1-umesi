import { Comment } from './comment'

export class Post {

    public id: number
    public userRef: string
    public title: string
    public description: string
    public ingredients: string[]
    public comments: Comment[]

    constructor(
         id: number,
         userRef: string,
         title: string,
         description: string,
         ingredients: string[],
         comments: Comment[]) { 

            this.id = id;
            this.userRef = userRef;
            this.title = title;
            this.description = description;
            this.ingredients = ingredients;
            this.comments = comments;
         }
}
