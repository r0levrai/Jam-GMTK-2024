import { Controller, Get, Post, Put, Delete, Param, Body, Query, Req } from "@nestjs/common";
import { DrawingService } from "./drawing.service";
import { Drawing } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";
import { Request } from 'express';



@Controller('drawings')
export class DrawingController {
    constructor(private readonly drawingService: DrawingService) {}

    @Post()
    async createDrawing(@Body() drawingDto: DrawingDto): Promise<Drawing> {
        return await this.drawingService.createDrawing(drawingDto);
    }

    @Get('lasts')
    async getLastDrawings(@Query('n') n: number){
        const drawings = await this.drawingService.getLastDrawings(n);
        return {drawings: drawings};
    }

    @Get('firsts')
    async getFirstDrawings(@Query('n') n: number) {
        const drawings = await this.drawingService.getFirstDrawings(n);
        return {drawings: drawings};
    }

    @Get('random')
    async getRandomDrawings(@Query('n') n: number) {
        const drawings = await this.drawingService.getRandomDrawings(n);
        return {drawings: drawings};
    }

    @Get('user/:userName')
    async getDrawingsByUserName(@Param('userName') userName: string) {
        const drawings = await this.drawingService.getDrawingsByUserName(userName);
        return {drawings: drawings};

    }

    @Put(':id')
    async updateDrawing(@Param('id') id: number, @Body() updateData: Partial<DrawingDto>): Promise<Drawing> {
        return await this.drawingService.updateDrawing(id, updateData);
    }

    @Delete(':id')
    async deleteDrawing(@Param('id') id: number): Promise<void> {
        await this.drawingService.deleteDrawing(id);
    }

    
    @Post(':id/reactions')
    async addReaction(
        @Param('id') id: number,
        @Body('reaction') reaction: string,
        @Req() request: Request
    ): Promise<Drawing> {
        const ipAdress = request.ip;
        return await this.drawingService.addReaction(id, reaction, ipAdress);
    }
}