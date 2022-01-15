export class Comment {

    public userRef: string;
    public description: string;
    public isDeleted: number;

    constructor(userRef: string, description: string, isDeleted: number) {

        this.userRef = userRef;
        this.description = description;
        this.isDeleted = isDeleted;
    }
}