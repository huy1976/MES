import clsx from 'clsx';
import styles from './ContentContainer.module.scss'
const ContentContainer = ({ children, className = "" , style}) => {
    return (
      <div
        className={clsx(styles.contentContainer, className)} style={style}
      > 
        {children}
      </div>
    );
  };
  
  export default ContentContainer;
  