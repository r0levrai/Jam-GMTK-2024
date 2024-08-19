import { forwardRef, Module } from '@nestjs/common';
import { DrawingController } from './drawing.controller';
import { DrawingService } from './drawing.service';
import { DrawingRepository } from './drawing.repository';
import { TypeOrmExModule } from 'src/database/typeorm-ex.module';
import { ReactionModule } from 'src/reaction/reaction.module';
import { ReactionRepository } from 'src/reaction/reaction.repository';

@Module({
  imports: [TypeOrmExModule.forCustomRepository([DrawingRepository, ReactionRepository]), 
  ReactionModule],
  controllers: [DrawingController],
  providers: [DrawingService],
})
export class DrawingModule {}